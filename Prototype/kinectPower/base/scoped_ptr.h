#pragma once

template <class T>
class scoped_ptr {
 public:
  scoped_ptr() : ptr_() { }
  ~scoped_ptr();

  explicit scoped_ptr(T* p) : ptr_(p) { }

  T* get() const { return ptr_; }
  T* operator->() const { return ptr_; }
  void reset(T* p);
  T* release();

 private:
  T* ptr_;
};

template <typename T>
scoped_ptr<T>::~scoped_ptr() {
  delete ptr_;
}

template <typename T>
void scoped_ptr<T>::reset(T* p) {
  if (ptr_ == p)
    return;
  delete ptr_;
  ptr_ = p;
}

template <typename T>
T* scoped_ptr<T>::release() {
  T* previous_ptr = ptr_;
  ptr_ = NULL;
  return previous_ptr;
}
