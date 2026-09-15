"use strict";
const serverUrl = "";
let playerId = null;
const canvas = document.getElementById("gameCanvas");
const context = canvas.getContext("2d");
const tileSize = 20;
let maze = null;
document
    .getElementById("joinButton")
    .addEventListener("click", joinGame);
async function joinGame() {
    const input = document.getElementById("nameInput");
    const name = input.value.trim();
    if (!name)
        return;
    const response = await fetch(`${serverUrl}/join/${encodeURIComponent(name)}`, {
        method: "POST"
    });
    const player = await response.json();
    playerId = player.id;
    document.getElementById("playerName").textContent = name;
    document.getElementById("login").classList.add("hidden");
    document.getElementById("game").classList.remove("hidden");
    await loadMaze();
}
async function loadMaze() {
    const response = await fetch(`${serverUrl}/maze`);
    maze = await response.json();
}
document.addEventListener("keydown", async (event) => {
    if (playerId === null)
        return;
    let dx = 0;
    let dy = 0;
    switch (event.key.toLowerCase()) {
        case "w":
            dy = -1;
            break;
        case "s":
            dy = 1;
            break;
        case "a":
            dx = -1;
            break;
        case "d":
            dx = 1;
            break;
        case " ":
            event.preventDefault();
            await fetch(`${serverUrl}/shoot/${playerId}`, {
                method: "POST"
            });
            return;
    }
    if (dx !== 0 || dy !== 0) {
        await fetch(`${serverUrl}/move/${playerId}/${dx}/${dy}`, {
            method: "POST"
        });
    }
});
function drawMaze() {
    if (!maze)
        return;
    context.fillStyle = "#4a4f58";
    for (const wall of maze.walls) {
        context.fillRect(wall.position.x * tileSize, wall.position.y * tileSize, tileSize, tileSize);
    }
}
function drawTank(player) {
    const x = player.x * tileSize;
    const y = player.y * tileSize;
    const centerX = x + tileSize / 2;
    const centerY = y + tileSize / 2;
    const tank = player.tank;
    // Tanko korpusas
    context.fillStyle = "#3f8f5f";
    context.fillRect(x + 3, y + 5, tileSize - 6, tileSize - 8);
    // Bokštelis
    context.beginPath();
    context.arc(centerX, centerY, 4, 0, Math.PI * 2);
    context.fillStyle = "#65b87c";
    context.fill();
    // Vamzdis
    context.beginPath();
    context.moveTo(centerX, centerY);
    context.lineTo(centerX + tank.directionX * 10, centerY + tank.directionY * 10);
    context.strokeStyle = "#d4d4d4";
    context.lineWidth = 3;
    context.stroke();
    // Savo tankui nicko viršuje nerodom,
    // nes jis jau yra HUD'e
    if (player.id !== playerId) {
        context.fillStyle = "white";
        context.font = "12px Arial";
        context.textAlign = "center";
        context.fillText(player.name.substring(0, 10), centerX, y - 4);
    }
}
function drawProjectile(projectile) {
    const centerX = projectile.position.x * tileSize +
        tileSize / 2;
    const centerY = projectile.position.y * tileSize +
        tileSize / 2;
    context.beginPath();
    context.arc(centerX, centerY, 3, 0, Math.PI * 2);
    context.fillStyle = "white";
    context.fill();
}
async function updateGame() {
    if (playerId === null)
        return;
    const playersResponse = await fetch(`${serverUrl}/players`);
    const playersData = await playersResponse.json();
    const projectileResponse = await fetch(`${serverUrl}/projectiles`);
    const projectiles = await projectileResponse.json();
    context.clearRect(0, 0, canvas.width, canvas.height);
    drawMaze();
    for (const player of playersData.players) {
        drawTank(player);
    }
    for (const projectile of projectiles) {
        drawProjectile(projectile);
    }
}
setInterval(updateGame, 50);
