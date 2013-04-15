#pragma once

#include "IRemover.h"

/*
	Remove zombie locks across the repository, regardless of revision.
*/
class RepositoryZombieLockRemover :
	public IRemover
{
private:
	char* _repositoryPath;
	char* _repositorySubPath;


	// Metadata for subversion operations.
	apr_pool_t*		_memoryPool;
	svn_repos_t*	_svnRepository;
	svn_fs_t*		_svnRepositoryFileSystem;
	const char*		_svnFileSystemType;
	svn_fs_root_t*	_svnRevisionRoot;

	std::vector<svn_lock_t> _locks;

	static svn_error_t* LockDiscoveryCallback(void *RepositoryUnlocker, svn_lock_t *lock, apr_pool_t *pool);
	static svn_error_t* UnlockNonexistingFiles(void *RepositoryUnlocker, svn_lock_t *lock, apr_pool_t *callback_pool);

public:
	RepositoryZombieLockRemover(char* repositoryPath, char* repositorySubPath);
	virtual ~RepositoryZombieLockRemover(void);

	void GetDeletedPaths();
	virtual void Run();
};
