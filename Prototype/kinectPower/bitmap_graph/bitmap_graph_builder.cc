#include "bitmap_graph/bitmap_graph_builder.h"

namespace bitmap_graph {

BitmapGraphBuilder::BitmapGraphBuilder()
    : current_parent_(NULL) {
}

void BitmapGraphBuilder::ObserveComponentStart() {
  scoped_ptr<BitmapGraph> graph(new BitmapGraph());
  graphs_.push_back(graph.Pass());
  current_parent_ = NULL;

  // Intersection sentinelle.
  intersections_.push(NULL);
}

void BitmapGraphBuilder::ObserveComponentEnd() {
  current_parent_ = NULL;
}

void BitmapGraphBuilder::ObserveIntersectionStart(int index) {
  Node* node = new Node(index);
  AddNodeToCurrentParent(node);
  current_parent_ = node;
  intersections_.push(node);
}

void BitmapGraphBuilder::ObserveIntersectionEnd() {
  intersections_.pop();
  current_parent_ = intersections_.top();
}

void BitmapGraphBuilder::ObservePixel(int index) {
  Node* node = new Node(index);
  AddNodeToCurrentParent(node);
  current_parent_ = node;
}

void BitmapGraphBuilder::ObserveLeaf(int index) {
  Node* node = new Node(index);
  AddNodeToCurrentParent(node);
  current_parent_ = intersections_.top();
}

void BitmapGraphBuilder::AddNodeToCurrentParent(Node* node) {
  if (current_parent_ == NULL) {
    graphs_.back()->SetRoot(scoped_ptr<Node>(node));
  } else {
    current_parent_->AddChild(scoped_ptr<Node>(node));
  }
}

}  // namespace bitmap_graph