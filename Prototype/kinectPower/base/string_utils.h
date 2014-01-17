// Copyright 2014 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#ifndef BASE_STRING_UTILS_H_
#define BASE_STRING_UTILS_H_

#include <string>

namespace base {

std::wstring StringToWString(const std::string& string);
std::string WStringToString(const std::wstring& string);

// Returns true if str begins with |starting|, or false otherwise.
bool StringBeginsWith(const std::string& str, const std::string& starting);

// Returns true if str ends with |ending|, or false otherwise.
bool StringEndsWith(const std::string& str, const std::string& ending);

// Returns true if str begins with |starting|, or false otherwise.
bool StringBeginsWith(const std::wstring& str, const std::wstring& starting);

// Returns true if str ends with |ending|, or false otherwise.
bool StringEndsWith(const std::wstring& str, const std::wstring& ending);

// Escape the C special characters with a backslash.
// @param str the string to be escaped.
// returns a string escaped copy of |str|.
std::string StringEscapeSpecialCharacter(const std::string& str);

}  // namespace base

#endif  // BASE_STRING_UTILS_H_
