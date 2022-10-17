window.clearRect = (id) => {    
    let canvas = document.getElementById(id);
    let g = canvas.getContext("2d");

    g.clearRect(0, 0, canvas.width, canvas.height);
}

window.fillRect = (id, fillStyle, x, y, width, height) => {    
    let canvas = document.getElementById(id);
    let g = canvas.getContext("2d");
    
    g.fillStyle = fillStyle;
    g.fillRect(x, y, width, height);
}

