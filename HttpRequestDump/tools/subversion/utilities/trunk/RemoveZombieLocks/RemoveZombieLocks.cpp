// RemoveZombieLocks.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "windows.h"

class RemoveZombieLocks
{

public:
	RemoveZombieLocks()
	{
	}

	int Run(int argc, TCHAR* argv[])
	{
		char*	repositoryPath = NULL;
		char*	revisionOrAll = NULL;
		IRemover* remover;

		if( argc < 3)
			return UsageAndExit();

		// The Subversion API requires the repository path to be char*, but
		// Windows/VC++ only gives it to us as TCHAR* (or really, a wchar_t*),
		// so we end up doing some conversion, but we still need to manage
		// the memory.  (This would be a good place to use std::string.)
		if(sizeof(TCHAR) == sizeof(wchar_t))
		{
			repositoryPath = TCharToChar(argv[1]);
			revisionOrAll = TCharToChar(argv[2]);
		}
		else
		{
			int strlen = _tcslen(argv[1]) + 1;
			repositoryPath = new char[strlen];
			strncpy(repositoryPath, (char*)argv[1], strlen);

			strlen = _tcslen(argv[2]) + 1;
			revisionOrAll = new char[strlen];
			strncpy(revisionOrAll, (char*)argv[2], strlen);
		}

		// Check the entire repository?  Or just one revision?
		if(!_tcsicmp(argv[2], _T("all")))
		{
			remover = new RepositoryZombieLockRemover(repositoryPath, "");
		}
		else
		{
			remover = new RevisionZombieLockRemover(repositoryPath, revisionOrAll);
		}

		remover->Run();
		delete remover;

		delete[] repositoryPath;
		delete[] revisionOrAll;

		return 0;
	}

private:
	int UsageAndExit()
	{
		std::cerr << "Usage: RemoveZombieLocks REPO_PATH all\n";
		std::cerr << "-or-   RemoveZombieLocks REPO_PATH REVISION\n\n";
		std::cerr << "where REPO_PATH is the physical file path to the repository.\n";
		std::cerr << "      REVISION is a revision number.\n\n";
		std::cerr << "If all is specified, all revisions are checked for zombie\n";
		std::cerr << "locks.  If REVISION is specified, only that specific\n";
		std::cerr << "revision is checked.\n\n";
		return 1;
	}

	/*
		Converts a TCHAR* to char*, because the SVN library only deals
		with char*, and Microsoft only gives us TCHAR*

		Caller is responsible for freeing memory.
	*/
	char* TCharToChar(TCHAR* orig)
	{
		char* result = NULL;
		int length = _tcslen(orig) + 1;

		if(length > 0)
		{
			result = new char[length];
			for(int i = 0; i < length; ++i)
				result[i] = (char)orig[i];
		}

		return result;
	}
};


int _tmain(int argc, TCHAR* argv[])
{
	RemoveZombieLocks app;
	return app.Run(argc, argv);
}

