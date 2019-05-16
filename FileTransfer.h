#ifndef FileTransfer_h
#define FileTransfer_h

#include "Arduino.h"
#include <MCUFRIEND_kbv.h>
#include "TouchScreen.h"

class CFileTransfer
{
    public:
        CFileTransfer();
        bool Start(MCUFRIEND_kbv* thetftptr, TouchScreen* thetsptr, HardwareSerial* theserptr);

    private:
        MCUFRIEND_kbv* m_service_tftptr;
        TouchScreen*   m_service_touchptr;
        HardwareSerial* m_service_serptr;
        void ReadFile (String theFileName);

};

#endif
