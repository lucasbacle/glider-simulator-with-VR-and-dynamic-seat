//Requête JQuery
//https://jquery.com/download/

/*MULTIPLE HTTP REQUEST*/
var requests = new Array();     
function ProcessUrls()
     {
         requests = new Array(); //crée un tableau de requêtes
         var urls = new Array('lirePotentiometre','lireButton', 'lireGyro'); //contient le complément d'url pour accéder aux capteurs
         var texte_urls = new Array('valeurPotentiometre','valeurButton', 'valeurGyro'); //contient les identifiants des balises html
         
         for(i=0;i<urls.length;i++)
         {
             requests.push(new ProcessUrl(urls[i], texte_urls[i]));  
         }
     }

     function ProcessUrl(url, texte_urls)
     {
            var http = new XMLHttpRequest();
            http.onreadystatechange = function()
            {
                if (http.readyState == 4 && http.status == 200)
                {
                   document.getElementById(texte_urls).innerHTML = http.responseText;
                }
            };
            http.open("GET", url, true);
            http.send(null);
     }

setInterval(ProcessUrls, 40) ;//Appels de la fonction ProcessUrls toutes les 40ms => 25fps


