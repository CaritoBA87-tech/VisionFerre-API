## 🛠️ VisionFerre: E-Commerce Inteligente con IA

<p align="left">
  <img src="https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  &nbsp; &nbsp;
  <img src="https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white" alt="Angular" />
  &nbsp; &nbsp;
  <img src="https://img.shields.io/badge/AWS-232F3E?style=for-the-badge&logo=amazon-aws&logoColor=white" alt="AWS" />
</p>

VisionFerre es una ferretería en línea que resuelve el problema de identificar piezas técnicas (tornillería, abrazaderas, anclajes) mediante una imagen, utilizando Inteligencia Artificial.

[![Demo](https://img.shields.io/badge/VisiónFerre_en_vivo-FF9900?style=for-the-badge&logo=google-chrome&logoColor=white)](https://vision-ferre-front-end.vercel.app)

> **Nota:** Por motivos de optimización de costos en AWS, el modelo de **AWS Rekognition** y el asistente **AWS Bedrock** se encuentran pausados. Si desea una demostración en vivo de las capacidades de IA, no dude en contactarme para activarlos.

<br>


### 🚀 Características Principales
**Reconocimiento de Imágenes (IA)**: Los usuarios pueden subir una foto de una pieza física y el sistema utiliza **AWS Rekognition (Custom Labels)** para identificarla y mostrar productos coincidentes en el inventario.

**Asistente Inteligente (IA)**: un chatbot brinda detalles de las especificaciones de los productos disponibles en tiempo real con **AWS Bedrock**.

<br>

### ☁️ Stack Tecnológico

| Tecnología | Uso |
| :--- | :--- |
| **C# / .NET 8 / ADO.NET** | Desarrollo del Backend (Alojado en Railway) |
| **Angular** | Framework para la interfaz de usuario (Alojado en Vercel) | 
| **SQL Server** | Base de datos (Alojada en Somee) |
| **AWS Rekognition (Custom Labels)** | Modelo entrenado con 719 imágenes de piezas de ferretería y 47 etiquetas, con un 93.5% de certeza <kbd>(Actualmente offline para control de costos)</kbd> |
| **AWS Bedrock** | Asistente inteligente que proporciona las especificaciones de los productos disponibles -Modelo *Llama 3.2 1B Instruct*- <kbd> (Disponible bajo solicitud para demostraciones)</kbd> | 
| **AWS S3** | Almacenamiento para las imágenes de los productos |
| **AWS IAM** | Permisos requeridos para acceder a los servicios de AWS desde .NET |
| **Json Web Token** | Autenticación y autorización del usuario |
| **Bootstrap** | Interfaz visual |
| **Arquitectura Multicapa** | 4 capas: API, Lógica, Datos y Entidades |

<br>

### 📩 Contacto

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/carolina-bolayna-alvarez-b475b4ba)
[![Gmail](https://img.shields.io/badge/Gmail-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:carolinabolayna13@gmail.com)


**Carolina Bolayna Alvarez** 

*Desarrollador fullstack | AWS Certified Cloud Practitioner*

<br>

### 📸 Demostración Visual

<p align="center">
  <i><b>Página principal</b></i> <br> <br>
  <img src="assets/img/Screenshots/Home.png" alt="Home" />  
</p>


<p align="center">
  <br><i><b>Navegación por categorías de herramientas</b></i> <br> <br>
  <img src="assets/img/Screenshots/Menu.png" alt="Menu" />  
</p>


<p align="center">
  <br><i><b>Ejemplo de los tornillos autoperforantes</b></i> <br> <br>
  <img src="assets/img/Screenshots/Autoperforantes.png" alt="Autoperforantes" />  
</p>

<p align="center">
  <br><i><b>Filtros para acotar los resultados</b></i> <br> <br>
  <img src="assets/img/Screenshots/Filtros.png" alt="Filtros" />  
</p>

<p align="center">
  <br><i><b>Detalle del producto</b></i> <br> <br>
  <img src="assets/img/Screenshots/Detalle.png" alt="Detalle" />  
</p>

<p align="center">
  <br><i><b>Asistente inteligente con Amazon Rekognition que reconoce la pieza con base en una imagen proporcionada por el usuario</b><br>Ejemplo: Tornillo para metal con cabeza de cruz</i>  <br> <br>
  <img src="assets/img/Screenshots/TornilloMetalCruz1.png" alt="Tornillo para metal cruz" />  
</p>

<p align="center">
  <br><i><b>Asistente inteligente con Amazon Bedrock que pregunta sobre las especificaciones de la pieza buscada</b><br> Ejemplo: Tornillo para metal con cabeza de cruz</i>  <br> <br>
  <img src="assets/img/Screenshots/TornilloMetalCruz2.png" alt="Tornillo para metal cruz" />  
</p>

<p align="center">
  <br><i><b>Asistente inteligente con Amazon Bedrock que pregunta sobre las especificaciones de la pieza buscada</b><br> Ejemplo: Tornillo para metal con cabeza de cruz</i>  <br> <br>
  <img src="assets/img/Screenshots/TornilloMetalCruz3.png" alt="Tornillo para metal cruz" />  
</p>

<p align="center">
  <br><i><b>Asistente inteligente con Amazon Rekognition que reconoce la pieza con base en una imagen proporcionada por el usuario</b><br> Ejemplo: Rondana plana</i>  <br> <br>
  <img src="assets/img/Screenshots/RondanaPlana1.png" alt="Rondana plana" />  
</p>

<p align="center">
  <br><i><b>Asistente inteligente con Amazon Bedrock que pregunta sobre las especificaciones de la pieza buscada</b><br> Ejemplo: Rondana plana</i>  <br> <br>
  <img src="assets/img/Screenshots/RondanaPlana2.png" alt="Rondana plana" />  
</p>

<p align="center">
  <br><i><b>Carrito de compras que se guarda en el Local Storage del navegador</b></i>  <br> <br>
  <img src="assets/img/Screenshots/Carrito.png" alt="Carrito de compras" />  
</p>

<p align="center">
  <br><i><b>Autenticación con Json Web Token</b></i>  <br> <br>
  <img src="assets/img/Screenshots/Login.png" alt="Login" />  
</p>

<p align="center">
  <br><i><b>Generación de cookie de autenticación con Json Web Token</b></i>  <br> <br>
  <img src="assets/img/Screenshots/JWT.png" alt="Cookie de autenticación" />  
</p>

<p align="center">
  <br><i><b>Usuario autenticado</b></i>  <br> <br>
  <img src="assets/img/Screenshots/Autenticado.png" alt="Usuario autenticado" />  
</p>

<p align="center">
  <br><i><b>Finalizar compra</b></i>  <br> <br>
  <img src="assets/img/Screenshots/Finalizar.png" alt="Finalizar compra" />  
</p>

<p align="center">
  <br><i><b>Compra finalizada</b></i>  <br> <br>
  <img src="assets/img/Screenshots/Finalizado.png" alt="Compra finalizada" />  
</p>

<p align="center">
  <br><i><b>Otros ejemplos de herramientas que reconoce el modelo de Amazon Rekognition: </b> Tuerca de seguridad</i>  <br> <br>
  <img src="assets/img/Screenshots/Tuerca.png" alt="Tuerca de seguridad" />  
</p>

<p align="center">
  <br><i><b>Otros ejemplos de herramientas que reconoce el modelo de Amazon Rekognition: </b>Armella abierta</i>  <br> <br>
  <img src="assets/img/Screenshots/Armella.png" alt="Armella abierta" />  
</p>

<p align="center">
  <br><i><b>Otros ejemplos de herramientas que reconoce el modelo de Amazon Rekognition: </b>Chaveta en R</i>  <br> <br>
  <img src="assets/img/Screenshots/Chaveta.png" alt="Chaveta en R" />  
</p>

<p align="center">
  <br><i><b>Otros ejemplos de herramientas que reconoce el modelo de Amazon Rekognition: </b>Alcayata</i>  <br> <br>
  <img src="assets/img/Screenshots/Alcayata.png" alt="Alcayata" />  
</p>

<p align="center">
  <br><i><b>Otros ejemplos de herramientas que reconoce el modelo de Amazon Rekognition: </b>Tornillo de coche</i>  <br> <br>
  <img src="assets/img/Screenshots/TornilloCoche.png" alt="Tornillo de coche" />  
</p>

<br>

### 📩 Contacto

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/carolina-bolayna-alvarez-b475b4ba)
[![Gmail](https://img.shields.io/badge/Gmail-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:carolinabolayna13@gmail.com)


**Carolina Bolayna Alvarez** 

*Desarrollador fullstack | AWS Certified Cloud Practitioner*
