using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Phidgets;
using Phidgets.Events;

public class MotorData
{
    private MotorControl motorControl;

    public MotorControl device
    {
        get { return motorControl; }
    }

    public MotorData()
    {
        try
        {
            motorControl = new MotorControl();
            Open();
        }
        catch { }
    }

    public void Open()
    {
        if (device != null)
        {
            if (!device.Attached)
            {
                //interfaceKit.waitForAttachment(1000);
                MonoBehaviour.print("Motor Kit Attach");

                motorControl.Attach += new AttachEventHandler(motorControl_Attach);
                motorControl.Detach += new DetachEventHandler(motorControl_Detach);
                motorControl.Error += new ErrorEventHandler(motorControl_Error);

                motorControl.open(); //310019
                //motorControl.waitForAttachment(50);
            }
            else
                Debug.Log("already open");
        }
        else
        {
            Debug.LogWarning("Could not find the phidgets motor controller or drivers");
        }
    }

    public void Close()
    {
        if (null != motorControl)
        {
            motorControl.Attach -= motorControl_Attach;
            motorControl.Detach -= motorControl_Detach;
            motorControl.Error -= motorControl_Error;
        }
        //		MonoBehaviour.print (motorControl.motors [0].Velocity);
        //		
        //		motorControl.motors [0].Velocity = 100f;
        //
        //		MonoBehaviour.print (motorControl.motors [0].Velocity);
        //MonoBehaviour.print (motorControl);
        if (motorControl != null && motorControl.Attached)
        {
            //MonoBehaviour.print ("Trying close");
            //run any events in the message queue - otherwise close will hang if there are any outstanding events
            //Application.DoEvents();

            //			for (int i = 0; i < motorControl.motors.Count; i++)
            //			{
            //				motorControl.motors[i].Velocity = 0.0f; // Turn off the fan! 										
            //			}
            //			foreach (MotorControlMotor motor in motorControl.motors)
            //			{
            //				motor.Velocity = 0;
            //			}

            //When the application is being terminated, close the Phidget
            //	       	motorControl.Attach -= motorControl_Attach;
            //	        motorControl.Detach -= motorControl_Detach;
            //	        motorControl.Error -= motorControl_Error;

            for (int i = 0; i < motorControl.motors.Count; i++)
            {
                //motorControl.motors[i].Acceleration = 6250.00; // max acceleration
                motorControl.motors[i].Velocity = 0.0f;
            }

            try
            {
                //System.Threading.Thread.Sleep(2000);
                MonoBehaviour.print("Closing-------");

                motorControl.close();
                motorControl = null;
            }
            catch (PhidgetException pe)
            {
                Debug.Log("!!!!!!!!!!!" + pe.Description);
            }
        }
    }

    //attach event handler
    void motorControl_Attach(object sender, AttachEventArgs e)
    {
        //MotorControl attached = (MotorControl)sender;
        MonoBehaviour.print("Motor Control Open");
        //motorControl.open();

        //attached.DataRate = 100; 

        for (int i = 0; i < motorControl.motors.Count; i++)
        {
            motorControl.motors[i].Acceleration = 6250.00f; // max acceleration
            motorControl.motors[i].Velocity = 0.00f;
        }
    }
    //attach event handler
    void motorControl_Detach(object sender, DetachEventArgs e)
    {
        MonoBehaviour.print("Motor Kit Close");
        motorControl = null;
    }

    //attach event handler
    void motorControl_Error(object sender, ErrorEventArgs e)
    {
        Debug.Log("Phidgets display error: " + e.ToString());
    }
}
