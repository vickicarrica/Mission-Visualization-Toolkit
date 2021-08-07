
## Table of Contents

<details open="open">
  <summary>Contents</summary>
  <ol>
    <li>
      <a href="#website-link">Website Link</a>
    </li>
    <li>
      <a href="#welcome">Welcome</a>
    </li>
    <li>
      <a href="#about-us">About Us</a>
      <ul>
        <li><a href="#contact-us">Contact Us</a></li>
      </ul>
    </li>
   <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
      </ul>
     <ul>
        <li><a href="#adding-the-project-to-unity">Adding The Project To Unity</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#contributions">Contributions</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>

![MVT_Logo](https://user-images.githubusercontent.com/66328989/128603431-6cd6febe-86e1-4994-894b-efb5ffda8cd5.png)

## Website Link

To view the project navigate to **[this link](https://vickicarrica.github.io/Mission-Visualization-Toolkit/Builds/fourteen/page.html)**.

## Welcome

This Github Repository contains the source code that is used for the Mission Visualization Toolkit (MVT), a program that simplifies visualization for the Space Communications and Navigations (SCaN) network in 3D space. This project was developed as a part of a NASA internship opportunity. 

## About Us

Hello! We are a team of five high schoolers who were brought together by NASA and spent our summer working on this visualization.
Our names are Aman Garg (Gilman School, class of 2024), Arya Kazemnia (Gilman School, class of 2024), Leo Wang (Gilman School, class of 2024), Vicki Carrica (Old Saybrook High School, class of 2023), and Zoe Schoeneman-Frye (Montgomery Blair High School, class of 2022). Aman, Arya, and Leo are participating in an Experiential Learning Opportunity (ELO) this summer due to not meeting the age requirements to be official NASA interns, while Vicki and Zoe are both participating in their first internships at NASA. 

### Contact Us

If you have any questions regarding this project, feel free to connect with Vicki Carrica at vickicarrica@yahoo.com and she can put you in contact with whoever is most likely to be able to help you. 

## Getting Started

### Prerequisites

* Unity should be installed on your device before opening the project.
* This project currently works under version 2020.3.8f1 of Unity.
* WebGL Build Support should be installed as a platform.
 - To do this, go to Installs in Unity Hub.
 - Under the 2020.3.8f1 version, click Add Modules.
 - Find WebGL and install.
* Github Desktop should be installed on your device

### Adding The Project To Unity

This project was built on Unity. To be able to see it on that platform, perform the following steps:
* Begin by downloading all the project files to your computer. 
* Open the Unity Hub app.
* Select the "ADD" button under the projects tab.
* Select the corresponding project file on your computer.
* Make sure that the current Target Platform is WebGL and that the Unity Version is 2020.3.8f1.
* Open the project once added.

## Usage

MVT is built to extend the capabilities of STK and replace it in some capacity. It will allow SCaN systems to be visualized in an interactive space, one in which the perspective of the viewer could explore the 3D visualization with time manipulation as opposed to the video-like experience generated from STK. The operational cost of MVT is quite low, almost negligible, and it operates in real-time. To display STK in a browser is a time intensive and difficult task where MVT is less convoluted to make it more accessible to not only NASA employees but also NASA customers.

Upon opening MVT, you will see a screen with a start button. Click it to begin and be directed to the main view. Once the main view loads, you will be prompted to either build with the preset or build with a new scenario, for those with custom files. We recommend beginner users to start with the preset.

On the left toolbar, the fifth icon from the top, shaped as an "i", is the information button. For more specific and detailed information regarding MVT usage, please refer to that.

# Left Toolbar

All of the left tool bar buttons and their basic function, starting at the top and going down.
* Planet Creator: Used to create satellites.
* Navigation: Used to choose a reference frame, follow a specific object, as well as providing data about that object.
* JSON Save: Used to save your current scenario.
* JSON Load: Used to load in another scenario.
* Info: More information on how to use the visualization.

# Bottom Toolbar

All of the botton toolbar buttons and their basic function.
* Time Increment Textbox/Scroll: Used to set the amount of simulated time in seconds that pass during one real second.
* Play/Pause: Used to start and pause the simulation.
* Fast forward/backwards: Used to skip a certain amount of time into the future or past of the current time in the simulation.

## Contributions

We welcome contributions and additions! To make a change you must:
* Fork the repository.
* Make the change on Unity
* Build the project using WebGL. Select the folder ProjectName > Builds and add a new build number.
* Open Github Desktop and open the repository.
* Commit and push your changes onto Github
* To open a website of the changed project:
 - Go to settings in your repository
 - Go to Pages within Settings
 - Select branch as master under Source
 - Open the website at username.github.io/project-name/Builds/buildNumber/pageName.html (example url, replace fields with corresponding information)
 
 Please be conscious of crediting our work when changes are made.
 
## Acknowledgements
 
We'd like to begin by thanking NASA as a whole for the opportunity to intern this summer. The Goddard Space Flight center did a particularly good job of making our internships meaningful and exciting experiences. The Space Communications and Navigations (SCaN) team were incredibly welcoming and kind to us and aided us immensely during our project.
 
We cannot thank our mentor, George D. Bussey, enough for all of his help during this summer. He went above and beyond to provide an educational and worthwhile project. We would also like to thank our co-mentor, Elana Resnick, for helping those who attend Gilman school be a part of this project.

Finally, we would like to thank Jimmy Acevedo and Korine Powers for their role in making this summer so enjoyable.
 

