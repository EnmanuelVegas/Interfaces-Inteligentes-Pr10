# Interfaces Inteligentes. Práctica 10: Interfaces multimodales - Reconocimiento del habla

- **Enmanuel Vegas Acosta** (alu0101281698)
- **Práctica 10**: Interfaces multimodales - Reconocimiento del habla

## **Ejercicios propuestos**

_1. Crear una apk con el proyecto de whisper.tiny para Unity._

En primer lugar, debemos descargar el proyecto [Whisper](https://github.com/Macoron/whisper.unity) y generar el archivo apk después de verificar que todo funciona correctamente en el editor. La escena y [el script](./scripts/MicrophoneDemo.cs) que se han usado son los que vienen por defecto en el proyecto descargado. Se ha probado el reconocimiento del habla en 3 idiomas diferentes:

- Español:

![espanol](./images/ej1_espanol.gif)

- Inglés:

![ingles](./images/ej1_ingles.gif)

- Alemán:

![aleman](./images/ej1_aleman.gif)

_2. Crear una apk en la que se le dé al menos dos instrucciones por voz a algún objeto 3D._

En las líneas 130-255 de [este script](./scripts/WarriorWizardOrders.cs) se 
maneja el texto captado por el modelo, lo que permite aplicar ordenes sobre el 
comportamiento de los objetos, que podremos probar en nuestro ejecutable. En concreto,
tanto el "mago" como el "guerrero" podrán saltar y cambiar de color.

![guerrero_jump](./images/ej2_guerrero_jump.gif)

![warrior_change](./images/ej2_warrior_change.gif)

![mago_jump](./images/ej2_mago_jump.gif)