using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using GestureData;
namespace ClapHand1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        GestureRecog GesturerecogData;
        KinectSensor sensor;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

            
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors[0];
               
               this.sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
               this.sensor.SkeletonStream.Enable();
               //this.sensor.DepthStream.Range = DepthRange.Near;
               GesturerecogData = new GestureRecog();
               GesturerecogData.GestureType = GestureType.HandsClapping;
               GesturerecogData.GestureRecognied += GesturerecogData_GestureRecognied;
               this.sensor.SkeletonFrameReady += sensor_SkeletonFrameReady;
               this.sensor.DepthFrameReady += sensor_DepthFrameReady;
               this.sensor.Start();

            }
            else
            {
                MessageBox.Show("Not Connected");
                this.Close();
            }
        }

        void GesturerecogData_GestureRecognied(object sender, GestureEventArgs e)
        {
            MessageBox.Show("Clap");
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skel = e.OpenSkeletonFrame())
            {
                if (skel != null)
                {
                    Skeleton[] totalSkelton = new Skeleton[6];
                    skel.CopySkeletonDataTo(totalSkelton);
                    Skeleton fskel = (from trackskeleton in totalSkelton where trackskeleton.TrackingState == SkeletonTrackingState.Tracked select trackskeleton).FirstOrDefault();
                    if (fskel == null)
                    {
                        return;
                    }
                    if (fskel.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                    {
                        this.MapJointsWithUIElement(fskel);
                    }
                    if (fskel.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked)
                    {
                        this.MapJointsWithUIElement(fskel);
                    }
                    GesturerecogData.Skel = fskel;
                    GesturerecogData.StartRecog();
                }
            }
        }
        private void MapJointsWithUIElement(Skeleton sk)
        {
            DepthImagePoint dppoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(sk.Joints[JointType.HandRight].Position, DepthImageFormat.Resolution320x240Fps30);
            Canvas.SetLeft(righthand, dppoint.X);
            Canvas.SetTop(righthand, dppoint.Y);
            DepthImagePoint dppoint1 = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(sk.Joints[JointType.HandLeft].Position, DepthImageFormat.Resolution320x240Fps30);
            Canvas.SetLeft(lefthand, dppoint1.X);
            Canvas.SetTop(lefthand, dppoint1.Y);
        
        }
        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame dpimg = e.OpenDepthImageFrame())
            {
                if (dpimg != null)
                {
                    short[] pixelData = new short[dpimg.PixelDataLength];
                    int stride = dpimg.Width * 2;
                    dpimg.CopyPixelDataTo(pixelData);
                    DepthImage.Source = BitmapSource.Create(dpimg.Width, dpimg.Height, 96, 96, PixelFormats.Gray16, null, pixelData, stride);

                }
            }
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            if (this.sensor != null)
            {
                this.sensor.Stop();
            }
        }
    }
}
