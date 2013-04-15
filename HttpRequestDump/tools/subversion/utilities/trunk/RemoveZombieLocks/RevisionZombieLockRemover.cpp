#include "StdAfx.h"
#include "RevisionZombieLockRemover.h"

RevisionZombieLockRemover::RevisionZombieLockRemover(char* repositoryPath, char* repos_rev)
{
	_repositoryPath = new char[strlen(repositoryPath) + 1];
	strcpy(_repositoryPath, repositoryPath);

	_revisionNumber = atol(repos_rev);

	// Initialize SVN.
	apr_initialize();
	_memoryPool = svn_pool_create(NULL);

	// Open and initialize the repository.
	svn_error_t* status = svn_repos_open(&_svnRepository, _repositoryPath, _memoryPool);
	_svnRepositoryFileSystem = svn_repos_fs(_svnRepository);
	svn_fs_revision_root(&_svnRevisionRoot, _svnRepositoryFileSystem, _revisionNumber, _memoryPool);
}

RevisionZombieLockRemover::~RevisionZombieLockRemover(void)
{
	// Cleanup SVN.
	svn_pool_destroy(_memoryPool);
	apr_terminate();

	delete[] _repositoryPath;
}


/*
	Retrieve a list of paths which were deleted as part of the revision being scanned.
*/
svn_error_t* RevisionZombieLockRemover::GetDeletedPaths(std::vector<std::string>* deletedPaths)
{
	apr_hash_t* changedPaths = NULL;
	apr_hash_index_t*	index;

	const void* path;
	void* changeInfo;
	apr_ssize_t keySize;

	/*
		Retrieve an apr_hash_t containing a list of paths modified in the revision.
		The hash keys are const char* values containing the paths and the hash values
		are svn_fs_path_change2_t change type structures.
	*/
	SVN_ERR(svn_fs_paths_changed2(&changedPaths, _svnRevisionRoot, _memoryPool));

	/*
		Scan through the list of modified paths.  If the change_kind value is a
		deletion, add the path to the list.
	*/
	index = apr_hash_first(_memoryPool, changedPaths);
	while( index != NULL )
	{
		apr_hash_this(index, &path, &keySize, &changeInfo);

		if(((svn_fs_path_change2_t*)changeInfo)->change_kind == svn_fs_path_change_delete)
		{
			deletedPaths->push_back(std::string((char*)path));
		}

		index = apr_hash_next(index);
	}

	// No errors
	return NULL;
}


/*
	Remove any existing locks on files that are deleted in this revision.
*/
void RevisionZombieLockRemover::Run()
{
	std::vector<std::string> deletedPaths;
	svn_lock_t *lock;

	svn_error_t* error = GetDeletedPaths(&deletedPaths);
	if(error == NULL)
	{
		apr_pool_t* subpool = svn_pool_create(_memoryPool);

		/*
			Scan deleted paths for any which still have locks.
			Such undead locks are "zombies" and must be removed.
		*/
		for(std::vector<std::string>::iterator pathIterator = deletedPaths.begin();
			pathIterator != deletedPaths.end();
			pathIterator++)
		{
			// If the path is locked, unlock it.
			svn_pool_clear(subpool);
			svn_fs_get_lock(&lock, _svnRepositoryFileSystem, pathIterator->c_str(), subpool);
			if(lock != NULL)
			{
				error = svn_repos_fs_unlock(_svnRepository, lock->path, lock->token, TRUE, subpool);
				if(error)
				{
					std::cerr << "Error deleting Zombie lock on: " << lock->path << "\n";
					std::cerr << "In revisision " << _revisionNumber << "\n";
					std::cerr << error->message << "\n";
					exit(1);
				}
			}
		}

		svn_pool_destroy(subpool);
	}
	else
	{
		std::cerr << "Error checking revision " << _revisionNumber << " for zombie locks.\n";
		std::cerr << error->message;
		exit(1);
	}
}
