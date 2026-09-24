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

document
    .getElementById("joinButton")!
    .addEventListener("click", joinGame);


async function joinGame(): Promise<void> {

    const input =
        document.getElementById(
            "nameInput"
        ) as HTMLInputElement;

    const name = input.value.trim();

    if (!name)
        return;

    const response =
        await fetch(
            `${serverUrl}/join/${encodeURIComponent(name)}`,
            {
                method: "POST"
            });

    const player =
        await response.json();

    playerId = player.id;

    document.getElementById(
        "playerName"
    )!.textContent = name;

    document.getElementById(
        "login"
    )!.classList.add("hidden");

    document.getElementById(
        "game"
    )!.classList.remove("hidden");

    await loadMaze();
}


async function loadMaze(): Promise<void> {

    const response =
        await fetch(`${serverUrl}/maze`);

    maze = await response.json();
}


const heldKeys = new Set<string>();

let sentDrive = 0;
let sentTurn = 0;


document.addEventListener(
    "keydown",
    async event => {
        if (playerId === null)
            return;

        const key = event.key.toLowerCase();

        if (key === " ") {
            event.preventDefault();

            await fetch(
                `${serverUrl}/shoot/${playerId}`,
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

    context.save();
    context.translate(centerX, centerY);
    context.rotate(position.angle * Math.PI / 180);

    // Tanko korpusas
    context.fillStyle = isAlive ? "#3f8f5f" : "#3b3530";

    context.fillRect(
        -8,
        -6,
        16,
        12
    );

    // Bokštelis
    context.beginPath();

    context.arc(
        0,
        0,
        4,
        0,
        Math.PI * 2
    );

    context.fillStyle = isAlive ? "#65b87c" : "#5a4a3c";
    context.fill();

    // Vamzdis
    context.beginPath();

    context.moveTo(0, 0);
    context.lineTo(11, 0);

    context.strokeStyle = isAlive ? "#d4d4d4" : "#6b6b6b";
    context.lineWidth = 3;
    context.stroke();

    context.restore();

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


function drawBox(box: any): void {

    const x = box.position.x * tileSize;
    const y = box.position.y * tileSize;

    context.fillStyle = "#c8913a";

    context.fillRect(
        x + 3,
        y + 3,
        tileSize - 6,
        tileSize - 6
    );

    context.strokeStyle = "#7a5520";
    context.lineWidth = 2;

    context.strokeRect(
        x + 3,
        y + 3,
        tileSize - 6,
        tileSize - 6
    );
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

    if (playerId === null)
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

    if (playerId !== null) {

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
