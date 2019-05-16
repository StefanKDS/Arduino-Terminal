#include "FileTransfer.h"
#include "xmodem.h"
#include "SD.h"

CFileTransfer::CFileTransfer()
{
    
}

bool CFileTransfer::Start(MCUFRIEND_kbv* thetftptr, TouchScreen* thetsptr, HardwareSerial* theserptr) 
{
   if( thetftptr == NULL || thetsptr == NULL || theserptr == NULL )
        return false;

    m_service_tftptr = thetftptr;
    m_service_touchptr = thetsptr;
    m_service_serptr =theserptr;
    
    m_service_tftptr->fillScreen(BLACK);

    m_service_tftptr->setTextColor(WHITE);
    m_service_tftptr->setTextSize(2);
    m_service_tftptr->setCursor(5, 10);
    m_service_tftptr->print("File Transfer Modus");

    m_service_tftptr->setCursor(5, 30);
    m_service_tftptr->print("Warte auf Dateiname ...");

    delay(200);
    // Warte noch auf alte serielle Daten
    while(Serial.available())
    {
      int byte;
      byte = Serial.read();
    }

    int LoopCounter = 0;
    while(!Serial.available())
    {
      theserptr->println("Nothing available loop");
      delay(100);
      LoopCounter++;
      if( LoopCounter == 300 ) // 30 Sekunden
         break;
    }

    if( LoopCounter < 300 && Serial.available() )
    {
      String fileName;
      fileName = Serial.readString();   
      //m_service_serptr->flush();

      ReadFile (fileName); 
      
    }
    
    return true;
}

void CFileTransfer::ReadFile (String theFileName)
{
    m_service_tftptr->setTextColor(BLACK);
    m_service_tftptr->setTextSize(2);
    m_service_tftptr->setCursor(5, 30);
    m_service_tftptr->print("Warte auf Dateiname ...");

    m_service_tftptr->setTextColor(WHITE);
    m_service_tftptr->setTextSize(2);
    m_service_tftptr->setCursor(5, 30);
    m_service_tftptr->print("Empfange Daten...");

    char __dataFileName[theFileName.length()+1];
    theFileName.toCharArray(__dataFileName, sizeof(__dataFileName));

    m_service_serptr->print("Dateiname: ");       
    m_service_serptr->println(__dataFileName);

    if (XReceive( &SD,&Serial ,__dataFileName) == 0){
      m_service_tftptr->setTextColor(BLACK);
      m_service_tftptr->setTextSize(2);
      m_service_tftptr->setCursor(5, 30);
      m_service_tftptr->print("Empfange Daten...");

      m_service_tftptr->setTextColor(WHITE);
      m_service_tftptr->setTextSize(2);
      m_service_tftptr->setCursor(5, 30);
      m_service_tftptr->print("Fertig !");  

      delay (1000);
    }
}
