#pragma once

#include <vector>

#include "base/base.h"
#include "base/logging.h"

template <typename T>
class ObserverList {
public:
  typedef T ObserverType;
  typedef std::vector<ObserverType*> ObserverVector;
  typedef typename ObserverVector::iterator ObserverIterator;

  ObserverList() {}
  ~ObserverList() {}

  void AddObserver(T* observer) {
    observers_.push_back(observer);
  }

  ObserverIterator begin() {
    return observers_.begin();
  }
  ObserverIterator end() {
    return observers_.end();
  }

private:
  std::vector<T*> observers_;
};

#define FOR_EACH_OBSERVER(ObserverType, observer_list, func)               \
  {                                                                        \
    for (ObserverList<ObserverType>::ObserverIterator it =                 \
             observer_list.begin();                                        \
         it != observer_list.end(); ++it) {                                \
      (*it)->func;                                                         \
    }                                                                      \
  }
