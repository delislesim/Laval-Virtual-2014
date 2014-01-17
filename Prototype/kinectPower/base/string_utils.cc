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

#include "base/string_utils.h"

#include <sstream>

namespace base {

std::wstring StringToWString(const std::string& string) {
  return std::wstring(string.begin(), string.end());
}

std::string WStringToString(const std::wstring& string) {
  return std::string(string.begin(), string.end());
}

bool StringBeginsWith(const std::string& str, const std::string& starting) {
  if (str.compare(0, starting.length(), starting) == 0)
    return true;
  return false;
}

bool StringEndsWith(const std::string &str, const std::string &ending) {
  if (ending.length() > str.length())
    return false;

  if (str.compare(str.length() - ending.length(),
                  ending.length(),
                  ending) == 0) {
    return true;
  }

  return false;
}

bool StringBeginsWith(const std::wstring& str, const std::wstring& starting) {
  if (str.compare(0, starting.length(), starting) == 0)
    return true;
  return false;
}

bool StringEndsWith(const std::wstring &str, const std::wstring &ending) {
  if (ending.length() > str.length())
    return false;

  if (str.compare(str.length() - ending.length(),
                  ending.length(),
                  ending) == 0) {
    return true;
  }

  return false;
}

std::string StringEscapeSpecialCharacter(const std::string& str) {
  std::stringstream ss;
  for (std::string::const_iterator i = str.begin(); i != str.end(); ++i) {
    unsigned char c = *i;
    if (' ' <= c && c <= '~' && c != '\\' && c != '"') {
      ss << c;
      continue;
    }

    ss << '\\';

    switch (c) {
      case '"': ss << '"'; break;
      case '\\': ss << '\\'; break;
      case '\t': ss << 't'; break;
      case '\r': ss << 'r'; break;
      case '\n': ss << 'n'; break;
      default: {
        static char const* const hexdig = "0123456789ABCDEF";
        ss << 'x';
        ss << hexdig[c >> 4];
        ss << hexdig[c & 0xF];
        break;
      }
    }
  }

  return ss.str();
}

}  // namespace base

