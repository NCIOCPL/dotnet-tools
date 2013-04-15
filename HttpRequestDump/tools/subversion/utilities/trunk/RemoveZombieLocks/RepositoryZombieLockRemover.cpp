#include "StdAfx.h"
#include "RepositoryZombieLockRemover.h"

RepositoryZombieLockRemover::RepositoryZombieLockRemover(char* repositoryPath, char* repositorySubPath)
{
	_repositoryPath = new char[strlen(repositoryPath) + 1];
	_repositorySubPath = new char[strlen(repositorySubPath) + 1];

	strcpy(_repositoryPath, repositoryPath);
	strcpy(_repositorySubPath, repositorySubPath);

	// Initialize SVN.
	apr_initialize();
	_memoryPool = svn_pool_create(NULL);

	// Open and initialize the repository.
	svn_error_t* status = svn_repos_open(&_svnRepository, _repositoryPath, _memoryPool);
	_svnRepositoryFileSystem = svn_repos_fs(_svnRepository);
	svn_fs_type(&_svnFileSystemType, _repositoryPath, _memoryPool);

	svn_revnum_t youngestRevision;
	svn_fs_youngest_rev(&youngestRevision, _svnRepositoryFileSystem, _memoryPool);

	svn_fs_revision_root(&_svnRevisionRoot, _svnRepositoryFileSystem, youngestRevision, _memoryPool);
}

RepositoryZombieLockRemover::~RepositoryZombieLockRemover(void)
{
	// Cleanup SVN.
	svn_pool_destroy(_memoryPool);
	apr_terminate();

	delete[] _repositoryPath;
	delete[] _repositorySubPath;
}

// Callback method set up by RepositoryZombieLockRemover::Run() to
// record locks.
svn_error_t* RepositoryZombieLockRemover::LockDiscoveryCallback(void *RepositoryUnlocker, svn_lock_t *lock, apr_pool_t *pool)
{
	// Because the svn_lock_t structure contains pointers, we can't simply call push_back(*lock).
	// Instead, we need to make a deep copy, and push that back instead.  Happily, the Subversion
	// libraries give us a function for that.  Unhappily, the memory pool we receive in the
	// callback is temporary and gets cleared after LockDiscoveryCallback exits.  So in order to
	// make a copy of the lock which will still be valid in the next call, or even after the series
	// of callbacks is complete, we have to use the main memory pool.
	svn_lock_t* lockCopy = svn_lock_dup(lock, ((RepositoryZombieLockRemover *)RepositoryUnlocker)->_memoryPool);
	((RepositoryZombieLockRemover *)RepositoryUnlocker)->_locks.push_back(*lockCopy);
	return NULL;
}

// Callback method to check whether a locked file still exists in the HEAD revision
// and if not, remove the lock.
svn_error_t* RepositoryZombieLockRemover::UnlockNonexistingFiles(void *RepositoryUnlocker, svn_lock_t *lock, apr_pool_t *callback_pool)
{
	svn_node_kind_t nodeKind;
	svn_error_t* error = NULL;

	svn_fs_check_path(&nodeKind, ((RepositoryZombieLockRemover *)RepositoryUnlocker)->_svnRevisionRoot, lock->path, callback_pool);
	if( nodeKind == svn_node_none )
	{
		std::cout << "Removing lock for: " << lock->path << '\n';

		if((error = 
			svn_repos_fs_unlock(((RepositoryZombieLockRemover *)RepositoryUnlocker)->_svnRepository, lock->path, lock->token, TRUE, callback_pool)) != NULL)
		{
			std::cerr << error->message;
		}
	}

	return error;
}

void RepositoryZombieLockRemover::Run()
{
	std::cout << "Removing all zombie locks from repository at " << _repositoryPath << "\n";
	std::cout << "This may take several minutes...\n";

    // Subversion's Berkeley DB implementation doesn't allow reentry of
    // its BDB-transaction-using APIs from within BDB-transaction-using
    // APIs (such as svn_fs_get_locks()).  So we have do this in two
    // steps, first harvest the locks, then checking/removing them.
	if(!strcmp(_svnFileSystemType, SVN_FS_TYPE_BDB))
	{
		// Gather all locks currently existing in the repository
		svn_fs_get_locks(_svnRepositoryFileSystem, _repositorySubPath, LockDiscoveryCallback, (void*)this, _memoryPool);

		// Walk the list of locks, removing the ones which no longer
		// exist in the head revision.
		apr_pool_t* subpool = svn_pool_create(_memoryPool);
		for(std::vector<svn_lock_t>::iterator lockIterator = _locks.begin();
			lockIterator != _locks.end();
			lockIterator++ )
		{
			svn_pool_clear(subpool);
			UnlockNonexistingFiles((void*)this, &(*lockIterator), subpool);
		}
		svn_pool_destroy(subpool);
	}
	else
	{
		svn_fs_get_locks(_svnRepositoryFileSystem, _repositorySubPath, UnlockNonexistingFiles, (void*)this, _memoryPool);
	}

	std::cout<< "Done\n";
}