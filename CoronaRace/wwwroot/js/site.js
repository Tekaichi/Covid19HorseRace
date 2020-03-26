// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.


function toggleDeaths() {

    let deaths = document.getElementsByClassName("deaths");
    for (let i = 0; i < deaths.length;i++) {
        
        deaths[i].hidden = !deaths[i].hidden;
        
    }

 
}


function toggleActives() {

    let actives = document.getElementsByClassName("active");
    for (let i = 0; i < deaths.active; i++) {

        actives[i].hidden = !actives[i].hidden;

    }


}