
#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers

// Windows Header Files
#include <windows.h>
//#include <ShellAPI.h>
#include <Shlobj.h>


#include <assert.h>
#include <iostream>
#include <NuiApi.h>

extern "C" int __declspec(dllexport)
test() {
	std::cout << "coucou" << std::endl;
	return 3;
}