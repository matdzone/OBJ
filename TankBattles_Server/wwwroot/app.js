"use strict";
const serverUrl = "";
let playerId = null;
const canvas = document.getElementById("gameCanvas");
const context = canvas.getContext("2d");
const tileSize = 20;
let maze = null;
let gameActive = false;
let playerReady = false;
let lobbyInterval = null;
document
    .getElementById("joinButton")
    .addEventListener("click", joinGame);
document
    .getElementById("readyButton")
    .addEventListener("click", toggleReady);
document
    .getElementById("startGameButton")
    .addEventListener("click", startLobbyGame);
document
    .getElementById("changeGameModeButton")
    .addEventListener("click", changeGameMode);
async function joinGame() {
    const input = document.getElementById("nameInput");
    const tankSelect = document.getElementById("tankType");
    const name = input.value.trim();
    const tankType = tankSelect.value.trim();
    if (!name || !tankType)
        return;
    const response = await fetch(`${serverUrl}/join/${encodeURIComponent(name)}/${encodeURIComponent(tankType)}`, {
        method: "POST"
    });
    if (!response.ok)
        return;
    const player = await response.json();
    playerId = player.id;
    document
        .getElementById("playerName")
        .textContent = name;
    document
        .getElementById("lobbyPlayerName")
        .textContent = name;
    document
        .getElementById("login")
        .classList.add("hidden");
    document
        .getElementById("lobby")
        .classList.remove("hidden");
    await updateLobby();
    lobbyInterval =
        window.setInterval(updateLobby, 500);
}
async function toggleReady() {
    if (playerId === null)
        return;
    playerReady = !playerReady;
    const response = await fetch(`${serverUrl}/ready/${playerId}/${playerReady}`, {
        method: "POST"
    });
    if (!response.ok) {
        playerReady = !playerReady;
        return;
    }
    updateReadyButton();
    await updateLobby();
}
function updateReadyButton() {
    const readyButton = document.getElementById("readyButton");
    readyButton.textContent =
        playerReady
            ? "Not ready"
            : "Ready";
}
async function changeGameMode() {
    if (playerId === null)
        return;
    const select = document.getElementById("gameModeSelect");
    const gameMode = select.value;
    const response = await fetch(`${serverUrl}/lobby/mode/${playerId}/${encodeURIComponent(gameMode)}`, {
        method: "POST"
    });
    if (!response.ok) {
        console.log("Could not change game mode");
        return;
    }
    await updateLobby();
}
async function startLobbyGame() {
    if (playerId === null)
        return;
    const response = await fetch(`${serverUrl}/lobby/start/${playerId}`, {
        method: "POST"
    });
    if (!response.ok)
        return;
    await updateLobby();
}
async function updateLobby() {
    if (playerId === null)
        return;
    const response = await fetch(`${serverUrl}/lobby`);
    if (!response.ok)
        return;
    const lobby = await response.json();
    document
        .getElementById("gameModeValue")
        .textContent = lobby.gameMode;
    const gameModeSelect = document.getElementById("gameModeSelect");
    gameModeSelect.value =
        lobby.gameMode;
    const playerList = document.getElementById("lobbyPlayers");
    playerList.innerHTML = "";
    for (const player of lobby.players) {
        const item = document.createElement("li");
        const name = document.createElement("span");
        const status = document.createElement("span");
        name.textContent =
            player.id === lobby.hostId
                ? `${player.name} (Host)`
                : player.name;
        status.textContent =
            player.isReady
                ? "Ready"
                : "Not ready";
        item.appendChild(name);
        item.appendChild(status);
        playerList.appendChild(item);
        if (player.id === playerId) {
            playerReady = player.isReady;
            updateReadyButton();
        }
    }
    const isHost = lobby.hostId === playerId;
    document
        .getElementById("hostControls")
        .classList.toggle("hidden", !isHost);
    const startButton = document.getElementById("startGameButton");
    startButton.disabled =
        !lobby.canStart;
    document
        .getElementById("lobbyStatus")
        .textContent =
        lobby.canStart
            ? "Ready to start"
            : `Waiting for ${lobby.requiredPlayers} player(s) and ready status`;
    if (lobby.isStarted)
        startGame();
}
function startGame() {
    if (gameActive)
        return;
    gameActive = true;
    if (lobbyInterval !== null) {
        window.clearInterval(lobbyInterval);
        lobbyInterval = null;
    }
    document
        .getElementById("lobby")
        .classList.add("hidden");
    document
        .getElementById("game")
        .classList.remove("hidden");
    loadMaze();
}
async function loadMaze() {
    const response = await fetch(`${serverUrl}/maze`);
    maze =
        await response.json();
}
const heldKeys = new Set();
const shellMovementKeys = {
    "1": "Straight",
    "2": "Bouncing",
    "3": "Accelerating"
};
let sentDrive = 0;
let sentTurn = 0;
document.addEventListener("keydown", async (event) => {
    if (playerId === null || !gameActive)
        return;
    const key = event.key.toLowerCase();
    if (key === "u") {
        event.preventDefault();
        await fetch(`${serverUrl}/undo/${playerId}`, {
            method: "POST"
        });
        return;
    }
    if (key === "q") {
        event.preventDefault();
        await fetch(`${serverUrl}/split/${playerId}`, {
            method: "POST"
        });
        return;
    }
    if (key === " ") {
        event.preventDefault();
        await fetch(`${serverUrl}/shoot/${playerId}`, {
            method: "POST"
        });
        return;
    }
    const shellMovement = shellMovementKeys[key];
    if (shellMovement) {
        await fetch(`${serverUrl}/shell/${playerId}/${shellMovement}`, {
            method: "POST"
        });
        return;
    }
    heldKeys.add(key);
    await sendInput();
});
document.addEventListener("keyup", async (event) => {
    heldKeys.delete(event.key.toLowerCase());
    await sendInput();
});
window.addEventListener("blur", async () => {
    heldKeys.clear();
    await sendInput();
});
async function sendInput(force = false) {
    if (playerId === null)
        return;
    const drive = (heldKeys.has("w") ? 1 : 0) -
        (heldKeys.has("s") ? 1 : 0);
    const turn = (heldKeys.has("d") ? 1 : 0) -
        (heldKeys.has("a") ? 1 : 0);
    if (!force && drive === sentDrive && turn === sentTurn)
        return;
    sentDrive = drive;
    sentTurn = turn;
    await fetch(`${serverUrl}/input/${playerId}/${drive}/${turn}`, {
        method: "POST"
    });
}
setInterval(() => sendInput(true), 500);
function drawMaze() {
    if (!maze)
        return;
    context.fillStyle = "#4a4f58";
    for (const wall of maze.walls) {
        context.fillRect(wall.position.x * tileSize, wall.position.y * tileSize, tileSize, tileSize);
    }
}
function drawTank(player, position) {
    const centerX = position.x * tileSize;
    const centerY = position.y * tileSize;
    const isAlive = player.tank.isAlive;
    const scale = player.tank.spriteScale ?? 1.0;
    // const scale = 1.0;
    const aliveColor = player.tank.colorHex ?? "#3f8f5f";
    const turretStyle = player.tank.turretStyle ?? "standard";
    const deadColor = "#3b3530";
    const deadTurret = "#5a4a3c";
    const bodyWidth = 16 * scale;
    const bodyHeight = 12 * scale;
    const turretSize = 4 * scale;
    const barrelLength = 11 * scale;
    const barrelWidth = turretStyle === "wide" ? 4 :
        turretStyle === "medium" ? 3 :
            turretStyle === "slim" ? 2 : 3;
    context.save();
    context.translate(centerX, centerY);
    context.rotate(position.angle * Math.PI / 180);
    context.fillStyle = isAlive ? aliveColor : deadColor;
    context.fillRect(-bodyWidth / 2, -bodyHeight / 2, bodyWidth, bodyHeight);
    context.beginPath();
    context.arc(0, 0, turretSize, 0, Math.PI * 2);
    context.fillStyle = isAlive ? lighten(aliveColor) : deadTurret;
    context.fill();
    context.beginPath();
    context.moveTo(0, 0);
    context.lineTo(barrelLength, 0);
    context.strokeStyle = isAlive ? "#d4d4d4" : "#6b6b6b";
    context.lineWidth = barrelWidth;
    context.stroke();
    context.restore();
    if (isAlive && player.tank.shield > 0) {
        context.beginPath();
        context.arc(centerX, centerY, 12 * scale, 0, Math.PI * 2);
        context.strokeStyle = "rgba(90, 160, 255, 0.8)";
        context.lineWidth = 2;
        context.stroke();
    }
    let labelY = centerY - 14 * scale;
    if (!isAlive) {
        drawSkull(centerX, centerY - 18);
        labelY = centerY - 28;
    }
    if (player.id !== playerId) {
        context.fillStyle = "white";
        context.font = "12px Arial";
        context.textAlign = "center";
        context.fillText(player.name.substring(0, 10), centerX, labelY);
    }
}
function lighten(hex) {
    const n = parseInt(hex.slice(1), 16);
    const r = Math.min(255, ((n >> 16) & 0xff) + 40);
    const g = Math.min(255, ((n >> 8) & 0xff) + 40);
    const b = Math.min(255, (n & 0xff) + 40);
    return `rgb(${r},${g},${b})`;
}
function drawSkull(x, y) {
    context.fillStyle = "#f2f2f2";
    context.beginPath();
    context.arc(x, y, 6, 0, Math.PI * 2);
    context.fill();
    context.fillRect(x - 4, y + 3, 8, 5);
    context.fillStyle = "#16181d";
    context.beginPath();
    context.arc(x - 2.5, y, 1.8, 0, Math.PI * 2);
    context.arc(x + 2.5, y, 1.8, 0, Math.PI * 2);
    context.fill();
    context.beginPath();
    context.moveTo(x, y + 2);
    context.lineTo(x - 1, y + 4);
    context.lineTo(x + 1, y + 4);
    context.closePath();
    context.fill();
    context.fillRect(x - 2, y + 6, 1, 2);
    context.fillRect(x + 1, y + 6, 1, 2);
}
function drawProjectile(projectile, ageSeconds) {
    const centerX = (projectile.x + projectile.velocityX * ageSeconds) * tileSize;
    const centerY = (projectile.y + projectile.velocityY * ageSeconds) * tileSize;
    const isRocket = projectile.kind === "Rocket";
    context.beginPath();
    context.arc(centerX, centerY, isRocket ? 5 : 3, 0, Math.PI * 2);
    context.fillStyle =
        isRocket ? "#ff7a2f" : "white";
    context.fill();
}
const boxStyles = {
    Rocket: { fill: "#c8913a", stroke: "#7a5520" },
    Health: { fill: "#e8e8e8", stroke: "#9a9a9a" },
    Shield: { fill: "#3f7fd6", stroke: "#1f4b8a" },
    Disguise: { fill: "#8a2be2", stroke: "#491692" }
};
function drawBox(box) {
    const x = box.position.x * tileSize;
    const y = box.position.y * tileSize;
    const centerX = x + tileSize / 2;
    const centerY = y + tileSize / 2;
    const style = boxStyles[box.kind] ?? boxStyles.Rocket;
    context.fillStyle = style.fill;
    context.fillRect(x + 3, y + 3, tileSize - 6, tileSize - 6);
    context.strokeStyle = style.stroke;
    context.lineWidth = 2;
    context.strokeRect(x + 3, y + 3, tileSize - 6, tileSize - 6);
    if (box.kind === "Health") {
        context.fillStyle = "#d63b3b";
        context.fillRect(centerX - 1.5, centerY - 5, 3, 10);
        context.fillRect(centerX - 5, centerY - 1.5, 10, 3);
    }
    else if (box.kind === "Shield") {
        context.beginPath();
        context.arc(centerX, centerY, 4, 0, Math.PI * 2);
        context.strokeStyle = "white";
        context.lineWidth = 1.5;
        context.stroke();
    }
    else if (box.kind === "Disguise") {
        context.beginPath();
        context.ellipse(centerX, centerY, 7, 4, 0, 0, Math.PI * 2);
        context.fillStyle = "#f2e9ff";
        context.fill();
        context.fillStyle = style.fill;
        context.beginPath();
        context.ellipse(centerX - 3, centerY - 0.5, 1.8, 1.2, 0, 0, Math.PI * 2);
        context.fill();
        context.beginPath();
        context.ellipse(centerX + 3, centerY - 0.5, 1.8, 1.2, 0, 0, Math.PI * 2);
        context.fill();
    }
    else {
        context.beginPath();
        context.moveTo(centerX + 5, centerY);
        context.lineTo(centerX - 4, centerY - 3);
        context.lineTo(centerX - 4, centerY + 3);
        context.closePath();
        context.fillStyle = "#6b3a12";
        context.fill();
    }
}
const renderPositions = new Map();
let players = [];
let projectiles = [];
let boxes = [];
let projectilesReceivedAt = performance.now();
let lastFrameTime = performance.now();
async function updateGame() {
    if (playerId === null || !gameActive)
        return;
    const [playersData, projectileData, boxData] = await Promise.all([
        fetch(`${serverUrl}/players`).then(r => r.json()),
        fetch(`${serverUrl}/projectiles`).then(r => r.json()),
        fetch(`${serverUrl}/boxes`).then(r => r.json())
    ]);
    players = playersData.players;
    projectiles = projectileData;
    boxes = boxData;
    projectilesReceivedAt = performance.now();
    const me = players.find((player) => player.id === playerId);
    if (me) {
        document.getElementById("weaponName").textContent = me.tank.weapon.name;
        document.getElementById("shellName").textContent = me.tank.shellMovement;
        document.getElementById("healthValue").textContent =
            me.tank.shield > 0
                ? `${me.tank.health} +${me.tank.shield}`
                : `${me.tank.health}`;
    }
}
function updateRenderPosition(player, deltaSeconds) {
    const tank = player.tank;
    let position = renderPositions.get(player.id);
    if (!position ||
        Math.hypot(position.x - tank.x, position.y - tank.y) > 2) {
        position = { x: tank.x, y: tank.y, angle: tank.angle };
        renderPositions.set(player.id, position);
    }
    const blend = Math.min(1, deltaSeconds * 15);
    position.x += (tank.x - position.x) * blend;
    position.y += (tank.y - position.y) * blend;
    const angleDifference = ((tank.angle - position.angle) % 360 + 540) % 360 - 180;
    position.angle += angleDifference * blend;
    return position;
}
function render(time) {
    const deltaSeconds = Math.min((time - lastFrameTime) / 1000, 0.1);
    lastFrameTime = time;
    if (playerId !== null && gameActive) {
        context.clearRect(0, 0, canvas.width, canvas.height);
        drawMaze();
        for (const box of boxes) {
            drawBox(box);
        }
        const sortedPlayers = [...players].sort((a, b) => Number(a.tank.isAlive) - Number(b.tank.isAlive));
        for (const player of sortedPlayers) {
            drawTank(player, updateRenderPosition(player, deltaSeconds));
        }
        const projectileAge = Math.min((performance.now() - projectilesReceivedAt) / 1000, 0.1);
        for (const projectile of projectiles) {
            drawProjectile(projectile, projectileAge);
        }
    }
    requestAnimationFrame(render);
}
setInterval(updateGame, 50);
requestAnimationFrame(render);
