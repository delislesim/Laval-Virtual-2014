#include "bitmap_graph/bitmap_graph.h"

#include <sstream>

#include "bitmap_graph/bitmap_graph_builder.h"
#include "bitmap_graph/bitmap_run.h"
#include "gmock/gmock.h"
#include "gtest/gtest.h"

namespace bitmap_graph {

namespace {

class MockGraphVisitor {
 public:
  MockGraphVisitor() : indent_(0) {}

  void StartVisitGraph(BitmapGraph* /* graph */) {
    out_ << "Graphe:" << std::endl;
    indent_ += 2;
  }

  void EndVisitGraph(BitmapGraph* /* graph */) {
    indent_ -= 2;
    assert(indent_ == 0);

    out_ << "(Fin du graphe)" << std::endl;
  }

  void StartVisitIntersection(Node* node) {
    out_ << std::string(indent_, ' ') << "Intersection: " << node->index() << std::endl;
    indent_ += 2;
  }

  void EndVisitIntersection(Node* node) {
    indent_ -= 2;
    out_ << std::string(indent_, ' ') << "(Fin de l'intersection: " << node->index() << ")" << std::endl;
  }

  void VisitNode(Node* node) {
    out_ << std::string(indent_, ' ') << "Node: " << node->index() << std::endl;
  }

  void VisitLeaf(Node* node) {
    out_ << std::string(indent_, ' ') << "Leaf: " << node->index() << std::endl;
  }

  std::string GetResult() {
    return out_.str();
  }

 private:
  int indent_;
  std::stringstream out_;
};

}  // namespace

TEST(BitmapGraph, Bitmap4x4) {
  // Construire le graphe.
  unsigned char values[] = {1, 1, 0, 0,
                            0, 1, 0, 0,
                            0, 1, 0, 1,
                            0, 0, 1, 0};
  cv::Mat test_mat(4, 4, CV_8U, values);

  BitmapGraphBuilder builder;
  BitmapRun run;
  run.Run(&test_mat, &builder);

  // Visiter les graphes.
  MockGraphVisitor visitor;
  for (BitmapGraphBuilder::GraphIterator it = builder.GraphBegin();
       it != builder.GraphEnd(); ++it) {
    (*it)->DepthFirstVisitor(&visitor);
  }

  // Vérifier le résultat.
  std::string resultat = visitor.GetResult();
}

TEST(BitmapGraph, Bitmap8x8) {
  // Construire le graphe.
  unsigned char values[] = {1, 0, 0, 0, 0, 0, 0, 1, 
                            0, 1, 0, 0, 0, 0, 0, 1, 
                            0, 0, 1, 0, 1, 1, 1, 0, 
                            0, 0, 0, 1, 0, 0, 1, 0, 
                            0, 0, 0, 1, 0, 0, 1, 0, 
                            0, 0, 0, 0, 1, 0, 1, 0, 
                            0, 0, 0, 0, 1, 1, 0, 0, 
                            0, 0, 0, 1, 0, 0, 1, 0 };
  cv::Mat test_mat(8, 8, CV_8U, values);

  BitmapGraphBuilder builder;
  BitmapRun run;
  run.Run(&test_mat, &builder);

  // Visiter les graphes.
  MockGraphVisitor visitor;
  for (BitmapGraphBuilder::GraphIterator it = builder.GraphBegin();
       it != builder.GraphEnd(); ++it) {
    (*it)->DepthFirstVisitor(&visitor);
  }

  // Vérifier le résultat.
  std::string resultat = visitor.GetResult();

  ASSERT_STREQ("Graphe:\n  Node: 62\n  Intersection: 53\n    Leaf: 44\n    Node: 52\n    Leaf: 59\n    Node: 46\n    Node: 38\n    Intersection: 30\n      Node: 21\n      Node: 20\n      Intersection: 27\n        Node: 18\n        Node: 9\n        Leaf: 0\n        Leaf: 35\n      (Fin de l'intersection: 27)\n      Node: 22\n      Node: 15\n      Leaf: 7\n    (Fin de l'intersection: 30)\n  (Fin de l'intersection: 53)\n(Fin du graphe)\n",
                resultat.c_str());


}

}  // bitmap_graph