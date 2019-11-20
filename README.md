# BDPreparation
Inspiré de https://github.com/javascriptdezero/BattleDev-vscode, qui fait la même chose (en mieux, avec des smileys) pour le code en Javascript.

L'idée est de pouvoir exécuter/débugguer le code C# pour la BattelDev depuis VSCode.

* Copier le ZIP des exemples dans le répertoire exemples/
* Copier le contenu de l'éditeur par défaut dans Contest.cs
* Développer la solution. 
* Lorsque l'on lance le débugger, cela lance le programme dans Programme.cs. Celui-ci va alors exécuter 
le programme de Contest.cs en lui passant les différents cas de tests des exemples, et en comparant avec 
la sortie attendue. 
* Lorsque ça marche, faire un copier coller de Contest.cs dans l'éditeur du site.

Dans mon VSCode pour Linux, il a fallu que j'installe : 
* le plugin ms-vscode-csharp
* le framework dotnet

Have fun !
