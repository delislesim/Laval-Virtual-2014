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

#ifndef BASE_SCOPED_PTR_H_
#define BASE_SCOPED_PTR_H_

template <class T>
class scoped_ptr {
 private:
  // This class used a C++ trick to implement move-constructors semantic in
  // C++03 by using the intermediate type |RValueType|.
  // see http://en.wikibooks.org/wiki/More_C%2B%2B_Idioms/Move_Constructor
  struct RValueType {
    explicit RValueType(scoped_ptr* object) : object_(object) { }
    scoped_ptr* object_;
  };

  // Forbid copy constructor and assign operator.
  scoped_ptr(scoped_ptr&);
  scoped_ptr& operator=(scoped_ptr&);

 public:
  // Default constructor.
  scoped_ptr() : data_() { }
  ~scoped_ptr();

  // Constructor with a raw pointer.
  // Takes ownership of |p| and release it when leaving the declaration scope.
  // @param p pointer to an allocated memory thunk or NULL.
  explicit scoped_ptr(T* p) : data_(p) { }

  // Move constructor.
  // Takes ownership of the pointer into the r-value.
  // @param rvalue the r-value wrapper object which contains the raw pointer.
  scoped_ptr(RValueType rvalue) : data_(rvalue.object_->release()) {
  }

  template <typename U>
  scoped_ptr(scoped_ptr<U> other) : data_(other.release()) {
  }

  // Converts to a RValue.
  // Implicite conversion to an r-value for the move only semantic.
  operator RValueType() { return RValueType(this); }

  // Explicit converstion to an r-value.
  scoped_ptr Pass() { return scoped_ptr(RValueType(this)); }

  template <typename PassAsType>
  scoped_ptr<PassAsType> PassAs() {
    return scoped_ptr<PassAsType>(Pass());
  }

  // Returns the raws pointer inside this wrapper.
  T* get() const { return data_; }

  // Returns the raws pointer inside this wrapper
  T* operator->() const { return data_; }

  // Takes ownership of |p| and deletes the currently owned object, if any.
  // @param p pointer to an allocated memory thunk or NULL.
  void reset(T* p);

  // Move assignment.
  // Takes ownership of the pointer into the r-value.
  // @param rvalue the r-value wrapper object which contains the raw pointer.
  scoped_ptr& operator=(RValueType rvalue);

  template <typename U>
  scoped_ptr<T>& operator=(scoped_ptr<U> other) {
    reset(other.release());
    return *this;
  }

  // Release the ownership of the pointer and return the raw pointer.
  // The caller is reponsible to delete the memory used.
  T* release();

 private:
  // The pointer managed by this class.
  T* data_;
};

template <typename T>
scoped_ptr<T>::~scoped_ptr() {
  delete data_;
}

template <typename T>
void scoped_ptr<T>::reset(T* p) {
  // Seft asssignment.
  if (data_ == p)
    return;

  // Delete the old data.
  delete data_;

  // Assign the new data.
  data_ = p;
}

template <typename T>
scoped_ptr<T>& scoped_ptr<T>::operator=(RValueType rvalue) {
  reset(rvalue.object_->release());
  return *this;
}

template <typename T>
T* scoped_ptr<T>::release() {
  T* old_ptr = data_;
  data_ = 0;
  return old_ptr;
}

template <typename T>
scoped_ptr<T> make_scoped_ptr(T* ptr) {
  return scoped_ptr<T>(ptr);
}

#endif  // BASE_SCOPED_PTR_H_
