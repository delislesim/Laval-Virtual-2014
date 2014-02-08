#pragma once

#include <list>
#include <opencv2/core/core.hpp>

#include "base/base.h"
#include "base/scoped_ptr.h"

namespace bitmap_graph {

class Node;

typedef std::list<scoped_ptr<Node> > NodeList;
typedef NodeList::iterator NodeIterator;

class Node {
 public:
  Node(int index) : index_(index) {}

  int index() const {
    return index_;
  }

  bool is_intersection() const {
    return children_.size() > 1;
  }

  bool is_leaf() const {
    return children_.size() == 0;
  }

  void AddChild(scoped_ptr<Node> child) {
    children_.push_back(child.Pass());
  }

  NodeIterator ChildBegin() {
    return children_.begin();
  }
  NodeIterator ChildEnd() {
    return children_.end();
  }

  int GetNumberOfChildren() const {
    return children_.size();
  }

 private:
  DISALLOW_COPY_AND_ASSIGN(Node);

  int index_;
  NodeList children_;
};

class BitmapGraph {
 public:
  BitmapGraph();

  Node* root() const {
    return root_.get();
  }
  void SetRoot(scoped_ptr<Node> root) {
    root_ = root.Pass();
  }

  // Un visiteur doit avoir les méthodes suivantes:
  // - StartVisitGraph(BitmapGraph*)
  // - EndVisitGraph(BitmapGraph*)
  // - StartVisitIntersection(Node*)
  // - EndVisitIntersection(Node*)
  // - VisitNode(Node*)
  // - VisitLeaf(Node*)
  template <typename VisitorType>
  void DepthFirstVisitor(VisitorType* visitor) {
    assert(visitor);

    visitor->StartVisitGraph(this);

    std::stack<Node*> intersections;
    std::stack<int> intersections_num_children;
    intersections.push(NULL);
    intersections_num_children.push(-1);

    std::stack<Node*> noeuds_a_visiter;
    if (root_.get() != NULL)
      noeuds_a_visiter.push(root_.get());

    while (!noeuds_a_visiter.empty()) {
      Node* node = noeuds_a_visiter.top();
      noeuds_a_visiter.pop();

      if (node->is_intersection()) {
        visitor->StartVisitIntersection(node);
        intersections.push(node);
        intersections_num_children.push(node->GetNumberOfChildren());        
      } else if (node->is_leaf()) {
        visitor->VisitLeaf(node);
        --intersections_num_children.top();

        // Si on a visité tous les noeuds des intersections parentes...
        while (intersections_num_children.top() == 0) {
          visitor->EndVisitIntersection(intersections.top());
          intersections.pop();
          intersections_num_children.pop();
          --intersections_num_children.top();
        }
      } else {
        visitor->VisitNode(node);
      }

      // Ajouter tous les enfants à la pile des noeuds à visiter.
      for (NodeIterator it = node->ChildBegin(); it != node->ChildEnd(); ++it) {
        noeuds_a_visiter.push(it->get());
      }

    }

    visitor->EndVisitGraph(this);

  }

 private:
  DISALLOW_COPY_AND_ASSIGN(BitmapGraph);

  scoped_ptr<Node> root_;
};

}  // bitmap_graph