using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
namespace GestureData
{
   public class GestureRecog
    {
       public GestureRecog()
       { }
       public event EventHandler<GestureEventArgs> GestureRecognied;
       public event EventHandler<GestureEventArgs> GestureNotRecognied;
       public GestureType GestureType { get; set; }
       public Skeleton Skel{get;set;}
       
       public void StartRecog()
       {
       switch(this.GestureType)
       {
           case GestureType.HandsClapping:
               this.MatchHandClappingGesture(this.Skel);
               break;
           default:
               break;

       }
       }

       float prevDist = 0.0f;
       private void MatchHandClappingGesture(Skeleton skeleton)
       {
       if(skeleton ==null)
       {
       return;
       }
           if(skeleton.Joints[JointType.HandRight].TrackingState==JointTrackingState.Tracked && skeleton.Joints[JointType.HandLeft].TrackingState==JointTrackingState.Tracked)
           {
                float currDist = GetJointDistance(skeleton.Joints[JointType.HandRight],skeleton.Joints[JointType.HandLeft]);
                if (currDist < 0.1f && prevDist > 0.1f)
                {
                    if (this.GestureRecognied != null)
                    {
                        this.GestureRecognied(this, new GestureEventArgs(RecogniedResult.Success));
                    }
                }
                else
                {
                    this.GestureRecognied(this, new GestureEventArgs(RecogniedResult.Unknow));
                }
               prevDist =currDist;
           }
           }
       private float GetJointDistance(Joint fJoint, Joint sjoint)
       {
            float distanceX = fJoint.Position.X -sjoint.Position.X;
             float distanceY = fJoint.Position.Y -sjoint.Position.Y;
           float distanceZ =fJoint.Position.Z -sjoint.Position.Z;
           return (float)Math.Sqrt(Math.Pow(distanceX,2)+Math.Pow(distanceY,2)+Math.Pow(distanceZ,2));

       }

    }
}
