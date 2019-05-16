# Arduino-Terminal

Arduino Terminal is a Serial Monitor with the possibility to send files to an Arduino attached SD-Card reader.

Find here the examples how to use the FilesUpload function on Arduino.

#include "FileTransfer.h"
#include <SPI.h>

CFileTransfer   Service;

SD.begin(SD_CS)

   if(Serial.available())
   {
       byte byteRead = Serial.read();

       //Warte auf das Startsignal 'r' um Dateien zu empfangen
       if(byteRead==114)
       {
          Serial.flush();
          Service.Start(&main_tft, &main_ts, &Serial);
          SetPinMode();
          printMainMenu();
       }

       //Warte auf das Startsignal 'l' um eine Dateiliste der SD Karte per
       //Serial zu schicken
       if(byteRead==108)
       {
          File root;
          root = SD.open("/");
          Serial.println("");
          printDirectory(root);  
          Serial.println("");      
          SetPinMode();
       }

       //Warte auf das Startsignal 'd' um eine Datei von der SD Karte zu löschen
       if(byteRead==100)
       {
          File root;
          root = SD.open("/");
          Serial.println("");
          removeFileFromSD();  
          Serial.println("");      
          SetPinMode();
       }
   }


/*****************************************************************/
/*   SD karte                                                    */
/*****************************************************************/

void removeFileFromSD()
{
    Serial.println("Warte auf Dateiname...");
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
      Serial.println("|");
      delay(100);
      LoopCounter++;
      if( LoopCounter == 300 ) // 30 Sekunden
         break;
    }

    if( LoopCounter < 300 && Serial.available() )
    {
      String fileName;
      fileName = Serial.readString();   

      if (SD.exists(fileName)) {
          SD.remove(fileName);
      } else {
        Serial.print(fileName);
        Serial.println(" gibt es nicht.");
      }     
    }
}

void printDirectory(File dir) {
    dir.rewindDirectory();
  
    while(true) {

        File entry =  dir.openNextFile();
        if (! entry) {
            dir.rewindDirectory();
            break;
        }
        else 
            entry.close();  //<-- We need to close the file here

        Serial.println(entry.name());
   }
}
