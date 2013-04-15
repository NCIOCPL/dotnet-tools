#pragma once

class IRemover
{
public:
	virtual ~IRemover(){}
	virtual void Run() = NULL;
};