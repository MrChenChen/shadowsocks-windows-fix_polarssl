// CreateLinkFile.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"

#include <tchar.h>
#include <iostream>

#include <string>

#include <process.h>
#include <Windows.h>

#include <direct.h>  
#include <Shlwapi.h>

#include <shlobj.h>


#pragma comment(lib,"Shlwapi.lib")

using namespace std;

// szStartAppPath : 点击后启动的程序
// szAddCmdLine : 传给main函数的lpCmdLine
// szDestLnkPath : 快捷方式的保存路径
// szIconPath : 快捷方式显示的图标
#ifdef _UNICODE
typedef wstring tstring;
#else
typedef string tstring;
#endif

extern "C" _declspec(dllexport) bool _stdcall CreateLinkFile(LPCTSTR szStartAppPath, LPCTSTR szAddCmdLine, LPCTSTR szDestLnkPath, LPCTSTR szIconPath)
{
	HRESULT hr = CoInitialize(NULL);
	if (SUCCEEDED(hr))
	{
		IShellLink *pShellLink;
		hr = CoCreateInstance(CLSID_ShellLink, NULL, CLSCTX_INPROC_SERVER, IID_IShellLink, (void**)&pShellLink);
		if (SUCCEEDED(hr))
		{
			pShellLink->SetPath(szStartAppPath);
			tstring strTmp = szStartAppPath;
			int nStart = strTmp.find_last_of(_T("/\\"));
			pShellLink->SetWorkingDirectory(strTmp.substr(0, nStart).c_str());
			pShellLink->SetArguments(szAddCmdLine);
			if (szIconPath)
			{
				pShellLink->SetIconLocation(szIconPath, 0);
			}
			IPersistFile* pPersistFile;
			hr = pShellLink->QueryInterface(IID_IPersistFile, (void**)&pPersistFile);
			if (SUCCEEDED(hr))
			{
				hr = pPersistFile->Save(szDestLnkPath, FALSE);
				if (SUCCEEDED(hr))
				{
					return true;
				}
				pPersistFile->Release();
			}
			pShellLink->Release();
		}
		CoUninitialize();
	}
	return false;
}
