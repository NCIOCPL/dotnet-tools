// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

//#include <stdio.h>
#include <tchar.h>
#include <iostream>
#include <vector>


// Third-party includes
// Subversion includes are somewhat silly and use the system-include syntax for including
// the Apache Portable Runtime files.  That makes sense if *everything* you're doing needs
// access to the APR, but if you're only using APR on a single project, it means you still
// have to set up a system-level include path under Tools | Options.
// (Expand "Projects and Solutions", select "VC++ Directories" and choose "Include Files"
// from the Show directories for" dropdown.)
// The system-include syntax here is for consistency, lest anyone get the idea that
// modifying a third-party header file is a good idea.
#include <apr_general.h>
#include "svn_fs.h"
#include "svn_pools.h"
#include "svn_repos.h"

// Application includes
#include "IRemover.h"
#include "RepositoryZombieLockRemover.h"
#include "RevisionZombieLockRemover.h"
