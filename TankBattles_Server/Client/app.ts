const serverUrl = "";

let playerId: number | null = null;

const canvas =
    document.getElementById(
        "gameCanvas"
    ) as HTMLCanvasElement;

const context =
    canvas.getContext("2d")!;

const tileSize = 20;

let maze: any = null;
let gameActive = false;
let playerReady = false;
let lobbyInterval: number | null = null;

interface LobbyPlayer {
    id: number;
    name: string;
    isReady: boolean;
}

interface LobbyResponse {
    name: string;
    hostId: number | null;
    gameMode: string;
    isStarted: boolean;
    requiredPlayers: number;
    canStart: boolean;
    players: LobbyPlayer[];
}


document
    .getElementById("joinButton")!
    .addEventListener("click", joinGame);

document
    .getElementById("readyButton")!
    .addEventListener("click", toggleReady);

document
    .getElementById("startGameButton")!
    .addEventListener("click", startLobbyGame);

document
    .getElementById("changeGameModeButton")!
    .addEventListener("click", changeGameMode);


async function joinGame(): Promise<void> {
    const input =
        document.getElementById(
            "nameInput"
        ) as HTMLInputElement;

    const tankSelect =
        document.getElementById(
            "tankType"
        ) as HTMLSelectElement;

    const name = input.value.trim();
    const tankType = tankSelect.value.trim();

    if (!name || !tankType)
        return;

    const response =
        await fetch(
            `${serverUrl}/join/${encodeURIComponent(name)}/${encodeURIComponent(tankType)}`,
            {
                method: "POST"
            }
        );

    if (!response.ok)
        return;

    const player =
        await response.json();

    playerId = player.id;

    document
        .getElementById("playerName")!
        .textContent = name;

    document
        .getElementById("lobbyPlayerName")!
        .textContent = name;

    document
        .getElementById("login")!
        .classList.add("hidden");

    document
        .getElementById("lobby")!
        .classList.remove("hidden");

    await updateLobby();

    lobbyInterval =
        window.setInterval(
            updateLobby,
            500
        );
}


async function toggleReady(): Promise<void> {
    if (playerId === null)
        return;

    playerReady = !playerReady;

    const response =
        await fetch(
            `${serverUrl}/ready/${playerId}/${playerReady}`,
            {
                method: "POST"
            }
        );

    if (!response.ok) {
        playerReady = !playerReady;
        return;
    }

    updateReadyButton();

    await updateLobby();
}


function updateReadyButton(): void {
    const readyButton =
        document.getElementById(
            "readyButton"
        ) as HTMLButtonElement;

    readyButton.textContent =
        playerReady
            ? "Not ready"
            : "Ready";
}


async function changeGameMode(): Promise<void> {
    if (playerId === null)
        return;

    const select =
        document.getElementById(
            "gameModeSelect"
        ) as HTMLSelectElement;

    const gameMode = select.value;

    const response =
        await fetch(
            `${serverUrl}/lobby/mode/${playerId}/${encodeURIComponent(gameMode)}`,
            {
                method: "POST"
            }
        );

    if (!response.ok) {
        console.log("Could not change game mode");
        return;
    }

    await updateLobby();
}


async function startLobbyGame(): Promise<void> {
    if (playerId === null)
        return;

    const response =
        await fetch(
            `${serverUrl}/lobby/start/${playerId}`,
            {
                method: "POST"
            }
        );

    if (!response.ok)
        return;

    await updateLobby();
}


async function updateLobby(): Promise<void> {
    if (playerId === null)
        return;

    const response =
        await fetch(
            `${serverUrl}/lobby`
        );

    if (!response.ok)
        return;

    const lobby: LobbyResponse =
        await response.json();

    document
        .getElementById("gameModeValue")!
        .textContent = lobby.gameMode;
    const gameModeSelect =
        document.getElementById(
            "gameModeSelect"
        ) as HTMLSelectElement;

    gameModeSelect.value =
        lobby.gameMode;
    const playerList =
        document.getElementById(
            "lobbyPlayers"
        ) as HTMLUListElement;

    playerList.innerHTML = "";

    for (const player of lobby.players) {
        const item =
            document.createElement("li");

        const name =
            document.createElement("span");

        const status =
            document.createElement("span");

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

    const isHost =
        lobby.hostId === playerId;

    document
        .getElementById("hostControls")!
        .classList.toggle(
            "hidden",
            !isHost
        );

    const startButton =
        document.getElementById(
            "startGameButton"
        ) as HTMLButtonElement;

    startButton.disabled =
        !lobby.canStart;

    document
        .getElementById("lobbyStatus")!
        .textContent =
        lobby.canStart
            ? "Ready to start"
            : `Waiting for ${lobby.requiredPlayers} player(s) and ready status`;

    if (lobby.isStarted)
        startGame();
}


function startGame(): void {
    if (gameActive)
        return;

    gameActive = true;

    if (lobbyInterval !== null) {
        window.clearInterval(
            lobbyInterval
        );

        lobbyInterval = null;
    }

    document
        .getElementById("lobby")!
        .classList.add("hidden");

    document
        .getElementById("game")!
        .classList.remove("hidden");

    loadMaze();
}


async function loadMaze(): Promise<void> {
    const response =
        await fetch(
            `${serverUrl}/maze`
        );

    maze =
        await response.json();
}

const heldKeys = new Set<string>();

const shellMovementKeys: Record<string, string> = {
    "1": "Straight",
    "2": "Bouncing",
    "3": "Accelerating"
};

let sentDrive = 0;
let sentTurn = 0;


document.addEventListener(
    "keydown",
    async event => {
        if (playerId === null || !gameActive)
            return;

        const key = event.key.toLowerCase();
        if (key === "u") {
            event.preventDefault();

            await fetch(
                `${serverUrl}/undo/${playerId}`,
                {
                    method: "POST"
                }
            );

            return;
        }

         if (key === "q") {
            event.preventDefault();

            await fetch(
                `${serverUrl}/split/${playerId}`,
                {
                    method: "POST"
                });

            return;
        }

        if (key === " ") {
            event.preventDefault();

            await fetch(
                `${serverUrl}/shoot/${playerId}`,
                {
                    method: "POST"
                });

            return;
        }
        

            

        const shellMovement =
            shellMovementKeys[key];

        if (shellMovement) {

            await fetch(
                `${serverUrl}/shell/${playerId}/${shellMovement}`,
                {
                    method: "POST"
                });

            return;
        }

        heldKeys.add(key);

        await sendInput();
    });


document.addEventListener(
    "keyup",
    async event => {
        heldKeys.delete(event.key.toLowerCase());

        await sendInput();
    });


window.addEventListener(
    "blur",
    async () => {
        heldKeys.clear();

        await sendInput();
    });


async function sendInput(force: boolean = false): Promise<void> {

    if (playerId === null)
        return;

    const drive =
        (heldKeys.has("w") ? 1 : 0) -
        (heldKeys.has("s") ? 1 : 0);

    const turn =
        (heldKeys.has("d") ? 1 : 0) -
        (heldKeys.has("a") ? 1 : 0);

    if (!force && drive === sentDrive && turn === sentTurn)
        return;

    sentDrive = drive;
    sentTurn = turn;

    await fetch(
        `${serverUrl}/input/${playerId}/${drive}/${turn}`,
        {
            method: "POST"
        });
}


setInterval(
    () => sendInput(true),
    500
);


function drawMaze(): void {

    if (!maze)
        return;

    context.fillStyle = "#4a4f58";

    for (const wall of maze.walls) {

        context.fillRect(
            wall.position.x * tileSize,
            wall.position.y * tileSize,
            tileSize,
            tileSize
        );
    }
}


function drawTank(player: any, position: RenderPosition): void {

    const centerX = position.x * tileSize;
    const centerY = position.y * tileSize;

    const isAlive = player.tank.isAlive;
    const tankType = player.tank.appearance.type;

    context.save();
    context.translate(centerX, centerY);
    context.rotate(position.angle * Math.PI / 180);


    let bodyWidth = 16;
    let bodyHeight = 12;
    let turretSize = 4;
    let barrelLength = 11;

    if (tankType === "Heavy") {
    bodyWidth = 20;
    bodyHeight = 16;
    turretSize = 5;
    barrelLength = 13;
    }

    else if (tankType === "Light") {
    bodyWidth = 14;
    bodyHeight = 10;
    turretSize = 3;
    barrelLength = 10;
    }

    // Tanko korpusas
    context.fillStyle = isAlive ? "#3f8f5f" : "#3b3530";

    context.fillRect(
    -bodyWidth / 2,
    -bodyHeight / 2,
    bodyWidth,
    bodyHeight
    );

    // Bokštelis
    context.beginPath();

    context.arc(
    0,
    0,
    turretSize,
    0,
    Math.PI * 2
    );

    context.fillStyle = isAlive ? "#65b87c" : "#5a4a3c";
    context.fill();

    // Vamzdis
    context.beginPath();

    context.moveTo(0, 0);
    context.lineTo(barrelLength, 0);

    context.strokeStyle = isAlive ? "#d4d4d4" : "#6b6b6b";
    context.lineWidth = 3;
    context.stroke();

    context.restore();

    if (isAlive && player.tank.shield > 0) {

        context.beginPath();
        context.arc(centerX, centerY, 12, 0, Math.PI * 2);

        context.strokeStyle = "rgba(90, 160, 255, 0.8)";
        context.lineWidth = 2;
        context.stroke();
    }

    let labelY = centerY - 14;

    if (!isAlive) {
        drawSkull(centerX, centerY - 18);

        labelY = centerY - 28;
    }

    // Savo tankui nicko viršuje nerodom,
    // nes jis jau yra HUD'e
    if (player.id !== playerId) {

        context.fillStyle = "white";
        context.font = "12px Arial";
        context.textAlign = "center";

        context.fillText(
            player.name.substring(0, 10),
            centerX,
            labelY
        );
    }
}


function drawSkull(x: number, y: number): void {

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


function drawProjectile(projectile: any, ageSeconds: number): void {

    const centerX =
        (projectile.x + projectile.velocityX * ageSeconds) * tileSize;

    const centerY =
        (projectile.y + projectile.velocityY * ageSeconds) * tileSize;
    const isRocket =
        projectile.kind === "Rocket";

    context.beginPath();

    context.arc(
        centerX,
        centerY,
        isRocket ? 5 : 3,
        0,
        Math.PI * 2
    );

    context.fillStyle =
        isRocket ? "#ff7a2f" : "white";

    context.fill();
}


const boxStyles: Record<string, { fill: string, stroke: string }> = {
    Rocket: { fill: "#c8913a", stroke: "#7a5520" },
    Health: { fill: "#e8e8e8", stroke: "#9a9a9a" },
    Shield: { fill: "#3f7fd6", stroke: "#1f4b8a" }
};


function drawBox(box: any): void {

    const x = box.position.x * tileSize;
    const y = box.position.y * tileSize;

    const centerX = x + tileSize / 2;
    const centerY = y + tileSize / 2;

    const style =
        boxStyles[box.kind] ?? boxStyles.Rocket;

    context.fillStyle = style.fill;

    context.fillRect(
        x + 3,
        y + 3,
        tileSize - 6,
        tileSize - 6
    );

    context.strokeStyle = style.stroke;
    context.lineWidth = 2;

    context.strokeRect(
        x + 3,
        y + 3,
        tileSize - 6,
        tileSize - 6
    );

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


interface RenderPosition {
    x: number;
    y: number;
    angle: number;
}

const renderPositions =
    new Map<number, RenderPosition>();

let players: any[] = [];
let projectiles: any[] = [];
let boxes: any[] = [];

let projectilesReceivedAt = performance.now();

let lastFrameTime = performance.now();


async function updateGame(): Promise<void> {

    if (playerId === null || !gameActive)
        return;

    const [playersData, projectileData, boxData] =
        await Promise.all([
            fetch(`${serverUrl}/players`).then(r => r.json()),
            fetch(`${serverUrl}/projectiles`).then(r => r.json()),
            fetch(`${serverUrl}/boxes`).then(r => r.json())
        ]);

    players = playersData.players;
    projectiles = projectileData;
    boxes = boxData;

    projectilesReceivedAt = performance.now();

    const me =
        players.find(
            (player: any) => player.id === playerId
        );

    if (me) {
        document.getElementById(
            "weaponName"
        )!.textContent = me.tank.weapon.name;

        document.getElementById(
            "shellName"
        )!.textContent = me.tank.shellMovement;

        document.getElementById(
            "healthValue"
        )!.textContent =
            me.tank.shield > 0
                ? `${me.tank.health} +${me.tank.shield}`
                : `${me.tank.health}`;
    }
}


function updateRenderPosition(player: any, deltaSeconds: number): RenderPosition {

    const tank = player.tank;

    let position =
        renderPositions.get(player.id);

    if (!position ||
        Math.hypot(position.x - tank.x, position.y - tank.y) > 2) {

        position = { x: tank.x, y: tank.y, angle: tank.angle };

        renderPositions.set(player.id, position);
    }

    const blend =
        Math.min(1, deltaSeconds * 15);

    position.x += (tank.x - position.x) * blend;
    position.y += (tank.y - position.y) * blend;

    const angleDifference =
        ((tank.angle - position.angle) % 360 + 540) % 360 - 180;

    position.angle += angleDifference * blend;

    return position;
}


function render(time: number): void {

    const deltaSeconds =
        Math.min((time - lastFrameTime) / 1000, 0.1);

    lastFrameTime = time;

    if (playerId !== null && gameActive) {

        context.clearRect(
            0,
            0,
            canvas.width,
            canvas.height
        );

        drawMaze();

        for (const box of boxes) {
            drawBox(box);
        }

        const sortedPlayers =
            [...players].sort(
                (a: any, b: any) =>
                    Number(a.tank.isAlive) - Number(b.tank.isAlive)
            );

        for (const player of sortedPlayers) {
            drawTank(
                player,
                updateRenderPosition(player, deltaSeconds)
            );
        }

        const projectileAge =
            Math.min((performance.now() - projectilesReceivedAt) / 1000, 0.1);

        for (const projectile of projectiles) {
            drawProjectile(projectile, projectileAge);
        }
    }

    requestAnimationFrame(render);
}


setInterval(
    updateGame,
    50
);

requestAnimationFrame(render);
