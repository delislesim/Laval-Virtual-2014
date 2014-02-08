#pragma once

#include <list>
#include <opencv2/core/core.hpp>
#include <stack>

#include "base/base.h"
#include "base/scoped_ptr.h"
#include "bitmap_graph/bitmap_graph.h"

namespace bitmap_graph {


class BitmapGraphBuilder {
 public:
  typedef std::list<scoped_ptr<BitmapGraph> > GraphList;
  typedef GraphList::iterator GraphIterator;

  BitmapGraphBuilder();

  void ObserveComponentStart();
  void ObserveComponentEnd();

  void ObserveIntersectionStart(int index);
  void ObserveIntersectionEnd();

  void ObservePixel(int index);
  void ObserveLeaf(int index);

  GraphIterator GraphBegin() {
    return graphs_.begin();
  }

  GraphIterator GraphEnd() {
    return graphs_.end();
  }

 private:
  void AddNodeToCurrentParent(Node* node);

  std::list<scoped_ptr<BitmapGraph> > graphs_;

  Node* current_parent_;
  std::stack<Node*> intersections_;

  DISALLOW_COPY_AND_ASSIGN(BitmapGraphBuilder);
};


}  // bitmap_graph