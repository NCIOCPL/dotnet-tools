#pragma once

#include "IRemover.h"

/*
	Remove all locks on files deleted in a revision
*/
class RevisionZombieLockRemover :
	public IRemover
{
private:
	char* _repositoryPath;
	long _revisionNumber;

	// Metadata for subversion operations.
	apr_pool_t*		_memoryPool;
	svn_repos_t*	_svnRepository;
	svn_fs_t*		_svnRepositoryFileSystem;
	svn_fs_root_t*	_svnRevisionRoot;

	svn_error_t* GetDeletedPaths(std::vector<std::string>* deletedPaths);

public:
	RevisionZombieLockRemover(char* repositoryPath, char* repos_rev);
	virtual ~RevisionZombieLockRemover(void);

	virtual void Run();
};
