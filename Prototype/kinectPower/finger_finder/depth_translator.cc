#include "finger_finder/depth_translator.h"

#include <vector>

#include "kinect_wrapper/constants.h"
#include "kinect_wrapper/kinect_sensor.h"
#include "kinect_wrapper/kinect_wrapper.h"

namespace finger_finder {

void DepthTranslator(int sensor_index, const cv::Mat& depth_mat,
                     cv::Mat* depth_mat_color_coordinates) {
  assert(depth_mat.type() == CV_16U);
  assert(depth_mat_color_coordinates);

 const int kNumPixels = kinect_wrapper::kKinectDepthWidth * 
                        kinect_wrapper::kKinectDepthHeight;

  // Créer un tableau de valeurs de profondeur dans le format requis par
  // le coordinate mapper de la Kinect.

  // TODO(fdoray): Plutôt que de faire une copie de ce tableau, passer celui
  // qui existe déjà à la base.

  std::vector<NUI_DEPTH_IMAGE_PIXEL> depth_pixels(kNumPixels);
  const unsigned short* depth_ptr =
      reinterpret_cast<const unsigned short*>(depth_mat.ptr());
  
  for (int i = 0; i < kNumPixels; ++i) {
    depth_pixels[i].playerIndex = 0;
    depth_pixels[i].depth = depth_ptr[i];
  }

  // Buffer pour recevoir les profondeur en coordonnées de l'image couleur.
  std::vector<NUI_DEPTH_IMAGE_POINT> depth_points(kNumPixels);

  // Demander au Kinect SDK de faire la traduction entre les systèmes
  // de coordonnées de pixels.
  kinect_wrapper::KinectSensor* sensor =
      kinect_wrapper::KinectWrapper::instance()->GetSensorByIndex(sensor_index);

  sensor->MapColorFrameToDepthFrame(&depth_pixels[0], &depth_points[0]);

  // Copier le résultat dans une matrice OpenCV.
  *depth_mat_color_coordinates = cv::Mat(depth_mat.size(), CV_16U);

  unsigned short* depth_color_ptr =
      reinterpret_cast<unsigned short*>(depth_mat_color_coordinates->ptr());

  for (int i = 0; i < kNumPixels; ++i) {
    depth_color_ptr[i] = static_cast<unsigned short>(depth_points[i].depth);
  }
}

}  // namespace finger_finder
