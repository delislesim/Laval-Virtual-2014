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

#ifndef BASE_BASE_H_
#define BASE_BASE_H_

typedef char int8;
typedef unsigned char uint8;
typedef short int16;
typedef unsigned short uint16;
typedef int int32;
typedef unsigned int uint32;
typedef long long int64;
typedef unsigned long long uint64;

// Macro to disable the copy constructor and assignment operator.
// Must be placed with the private declarations of a class.
#define DISALLOW_COPY_AND_ASSIGN(ClassType)                                    \
  ClassType(const ClassType&);                                                 \
  void operator=(const ClassType&)                                             \

// Macro to annotate methods that are overriding virtual methods from a base
// class.
// Sample use:
//     virtual void foo() OVERRIDE;
#if defined(_MSC_VER)
#define OVERRIDE override
#elif defined(__clang__)
#define OVERRIDE override
#else
#define OVERRIDE
#endif

#endif  // BASE_BASE_H_
