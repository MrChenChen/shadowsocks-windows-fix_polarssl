// CreateLinkFile.cpp : ���� DLL Ӧ�ó���ĵ���������
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

// szStartAppPath : ����������ĳ���
// szAddCmdLine : ����main������lpCmdLine
// szDestLnkPath : ��ݷ�ʽ�ı���·��
// szIconPath : ��ݷ�ʽ��ʾ��ͼ��
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
