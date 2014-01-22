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

// Macro to disable the copy constructor and assignment operator.
// Must be placed with the private declarations of a class.
#define DISALLOW_COPY_AND_ASSIGN(ClassType)                                    \
  ClassType(const ClassType&);                                                 \
  void operator=(const ClassType&)                                             \

#endif  // BASE_BASE_H_
