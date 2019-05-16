#ifndef FileTransfer_h
#define FileTransfer_h

#include "Arduino.h"

class CFileTransfer
{
    public:
        CFileTransfer();
        bool Start(HardwareSerial* theserptr);

    private:
        HardwareSerial* m_service_serptr;
        void ReadFile (String theFileName);

};

#endif
