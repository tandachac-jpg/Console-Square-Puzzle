using System;
using System.Diagnostics;

namespace square_puzzle
{
    internal class Program
    {
        #region 1-Functions
        static int[,,] allorientations = new int[5, 5, 160]; // bütün parçaların tüm orientationları saklanır 
        static int[,] RotatePiece(int[,] piecearea) // 90 derece döndür
        {
            int[,] newarea = new int[5, 5];
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (piecearea[r, c] == 1) newarea[c, 4 - r] = 1;
                }
            }
            newarea = NormalizePiece(newarea, CountSquares(newarea)); // rotate edildikten sonra normalize edilir
            return newarea;
        }
        static int[,] ReversePiece(int[,] piecearea)
        {
            int[,] reversedarea = new int[5, 5];
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (piecearea[r, c] == 1) reversedarea[r, 4 - c] = 1;
                }
            }
            reversedarea = NormalizePiece(reversedarea, CountSquares(reversedarea)); // ters çevrildikten sonra normalize edilir
            return reversedarea;
        }
        static int[,] NormalizePiece(int[,] area, int squarecount)
        {
            int[,] coordinates = new int[squarecount, 2];
            int currentIdx = 0;
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (area[r, c] == 1)
                    {
                        coordinates[currentIdx, 0] = r;
                        coordinates[currentIdx, 1] = c;
                        currentIdx++;
                    }
                }
            }
            int minRow = 5;
            int minCol = 5;
            for (int i = 0; i < squarecount; i++)
            {
                if (coordinates[i, 0] < minRow)
                {
                    minRow = coordinates[i, 0];
                }
                if (coordinates[i, 1] < minCol)
                {
                    minCol = coordinates[i, 1];
                }
            }
            int[,] newNormalizedArea = new int[5, 5];
            for (int i = 0; i < squarecount; i++)
            {
                newNormalizedArea[coordinates[i, 0] - minRow, coordinates[i, 1] - minCol] = 1;
            }
            return newNormalizedArea;
        }
        static int CountSquares(int[,] area)
        {
            int counter = 0;
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (area[r, c] == 1) counter++;
                }
            }
            return counter;
        }
        static int[,,] GetAllOrientations(int[,] area, int squarecount) // tüm görünüm(orientation)leri hesapla
        {
            int[,,] orientations = new int[5, 5, 8];
            int currentOrientationCount = 0;
            int[,] currentPiece = NormalizePiece(area, squarecount);
            for (int reflectionIndex = 0; reflectionIndex < 2; reflectionIndex++)
            {
                if (reflectionIndex == 1)
                {
                    currentPiece = ReversePiece(area);
                }
                for (int rotationIndex = 0; rotationIndex < 4; rotationIndex++)
                {
                    for (int r = 0; r < 5; r++)
                        for (int c = 0; c < 5; c++)
                            orientations[r, c, currentOrientationCount] = currentPiece[r, c];

                    currentOrientationCount++;
                    currentPiece = RotatePiece(currentPiece);
                }
            }
            return orientations;
        }
        static int[,,] StoreOrientations(int[,,] allorientations, int[,,] orientations, int pieceindex)
        {
            for (int o = 0; o < 8; o++)
            {
                for (int r = 0; r < 5; r++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        allorientations[r, c, pieceindex * 8 + o] = orientations[r, c, o];
                    }
                }
            }
            return allorientations;
        }
        static bool CheckIfUnique(int[,,] allorientations, int pieceindex, int[,] neworientation)
        {
            int existingCount = pieceindex * 8; // daha önce saklanmış orientation sayısı
            for (int o = 0; o < existingCount; o++)
            {
                bool same = true;
                for (int r = 0; r < 5; r++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        if (allorientations[r, c, o] != neworientation[r, c])
                        {
                            same = false;
                            break;
                        }
                    }
                    if (!same) break;
                }
                if (same) return false; // aynı orientation bulundu > benzersiz değil
            }
            return true; // benzersiz
        }
        static void PrintPiece(string harf, int[,] area, int startX, int startY)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                for (int j = 0; j < 5; j++)
                {
                    if (area[i, j] == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;

                        Console.Write(harf + " ");
                        Console.ResetColor();
                    }
                    else
                        Console.Write(". ");
                }
            }
        }
        static int[,] CopyPiece(int[,,] pieces, int bag)
        {
            int[,] temp = new int[5, 5];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    temp[i, j] = pieces[i, j, bag];
                }
            }

            return temp;
        }
        static bool CanPlace(int[,] shape, int px, int py, char[,] baseScreen)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (shape[i, j] == 1)
                    {
                        int y = py + i;
                        int x = px + j;
                        if (y < 0 || y >= 20 || x < 0 || x >= 30)
                        {
                            return false;
                        }
                        if (baseScreen[y, x] != 'X')
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        static void ShowWarning(string message)
        {
            Console.Beep(800, 200); // kısa bip
            Console.SetCursorPosition(0, 22);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message.PadRight(50));
            Console.ResetColor();
        }
        static void DrawScreen(char[,] baseScreen, char[,] placedScreen)
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    if (placedScreen[i, j] != ' ')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(placedScreen[i, j] + " ");
                        Console.ResetColor();
                    }
                    else if (baseScreen[i, j] != '.')
                    {
                        if (baseScreen[i, j] == 'X')
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("X ");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(baseScreen[i, j] + " ");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.Write(". ");
                    }
                }
                Console.WriteLine();
            }
        }
        static int[,] Placement(int[,,] chosenpieces, int piecenumber)
        {
            Random random = new Random();
            int[,] gamescreen = new int[20, 30];
            for (int k = 0; k < piecenumber; k++)
            {
                bool placed = false;
                while (!placed)
                {
                    int x = random.Next(0, 26);
                    int y = random.Next(0, 16);
                    bool collision = false;
                    bool adjacent = (k == 0);
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (chosenpieces[i, j, k] == 1)
                            {
                                if (gamescreen[y + i, x + j] != 0)
                                {
                                    collision = true;
                                }
                                if (k > 0)
                                {
                                    if ((y + i > 0 && gamescreen[y + i - 1, x + j] != 0) ||
                                        (x + j > 0 && gamescreen[y + i, x + j - 1] != 0) ||
                                        (y + i < 19 && gamescreen[y + i + 1, x + j] != 0) ||
                                        (x + j < 29 && gamescreen[y + i, x + j + 1] != 0))
                                    {
                                        adjacent = true;
                                    }
                                }
                            }
                        }
                    }
                    if (!collision && adjacent)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                if (chosenpieces[i, j, k] == 1)
                                {
                                    gamescreen[y + i, x + j] = k + 1;
                                }
                            }
                        }
                        placed = true;
                    }
                }
            }
            return gamescreen;
        }
        static int GetShapeWidth(int[,] shape)
        {
            int maxCol = -1;
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (shape[r, c] == 1 && c > maxCol)
                        maxCol = c;
                }
            }
            return maxCol + 1;
        }
        static int GetShapeHeight(int[,] shape)
        {
            int maxRow = -1;
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (shape[r, c] == 1 && r > maxRow)
                        maxRow = r;
                }
            }
            return maxRow + 1;
        }


        #endregion
        #region 2-Main
        static void Main(string[] args)
        {
            char[,] baseScreen = new char[20, 30];
            char[,] placedScreen = new char[20, 30];

            int selectedPiece = -1;
            int pieceX = 0, pieceY = 0;
            int[,] currentShape = new int[5, 5];

            int round = 1;
            double Score = 0;//RoundScore = TotalSquares * (4 * Regularity)^4

            while (true)
            {
                Console.Clear();
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        baseScreen[i, j] = '.';
                        placedScreen[i, j] = ' ';
                    }
                }

                Random random = new Random();
                Console.CursorVisible = false;
                Console.SetBufferSize(120, 120);
                allorientations = new int[5, 5, 160];
                int r1, c1 = 0;
                string[] harfler = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T" };
                double roundScore = 0;
                //RoundScore = TotalSquares * (4 * Regularity)^4

                Console.WriteLine("Enter numbers between 2-12 with space.");
                string input = Console.ReadLine();
                string[] pieces = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int piecenumber = pieces.Length;
                bool[] piecePlaced = new bool[piecenumber];
                bool regularity_ok = false;
                while (piecenumber > 20)
                {
                    Console.WriteLine("Enter a max of 20 numbers.");
                    input = Console.ReadLine();
                    pieces = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    piecenumber = pieces.Length;
                }
                bool invalid = false;
                for (int i = 0; i < pieces.Length; i++)
                {
                    int value = Convert.ToInt32(pieces[i]);
                    if (value < 2 || value > 12) invalid = true;
                }
                while (invalid)
                {
                    Console.WriteLine("Invalid! Enter numbers between 2-12 with space.");
                    input = Console.ReadLine();
                    pieces = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    piecenumber = pieces.Length;
                    invalid = false;
                    for (int i = 0; i < pieces.Length; i++)
                    {
                        int value = Convert.ToInt32(pieces[i]);
                        if (value < 2 || value > 12) invalid = true;
                    }
                }
                int sum = 0;
                for (int i = 0; i < pieces.Length; i++) sum += Convert.ToInt32(pieces[i]);
                while (sum > 160)
                {
                    Console.WriteLine("Enter a max of 160 squares.");
                    input = Console.ReadLine();
                    pieces = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    piecenumber = pieces.Length;
                    sum = 0;
                    for (int i = 0; i < pieces.Length; i++) sum += Convert.ToInt32(pieces[i]);
                }
                int[,] gamescreen = new int[20, 30];
                int[,,] chosenpieces = new int[5, 5, piecenumber];
                int attempt = 0;
                for (int k = 0; k < piecenumber; k++)
                {
                    bool unique = false;
                    int squarecount = Convert.ToInt32(pieces[k]);
                    int[,] area = new int[5, 5];
                    while (!unique)
                    {
                        area = new int[5, 5];
                        int count = 0;
                        c1 = random.Next(0, 5);
                        r1 = random.Next(0, 5);
                        area[r1, c1] = 1;
                        count++;
                        attempt = 0;
                        while (squarecount > count)
                        {
                            attempt++;
                            int rnext = random.Next(0, 5);
                            int cnext = random.Next(0, 5);
                            if (area[rnext, cnext] == 0)
                            {
                                bool full = false;
                                if (rnext > 0 && area[rnext - 1, cnext] == 1) full = true;
                                else if (cnext > 0 && area[rnext, cnext - 1] == 1) full = true;
                                else if (rnext < 4 && area[rnext + 1, cnext] == 1) full = true;
                                else if (cnext < 4 && area[rnext, cnext + 1] == 1) full = true;

                                if (full)
                                {
                                    area[rnext, cnext] = 1;
                                    count++;
                                }
                            }
                        }
                        int[,,] orientations = GetAllOrientations(area, squarecount);
                        int[,] newarea = NormalizePiece(area, squarecount);
                        //  benzersizlik kontrolü (mevcut saklı orientationlarla)
                        unique = CheckIfUnique(allorientations, k, newarea);
                        if (unique)
                        {
                            // benzersizse kaydet
                            allorientations = StoreOrientations(allorientations, orientations, k);

                            int rnd = random.Next(0, 8);
                            int[,] arearesult = new int[5, 5];

                            for (int r = 0; r < 5; r++)
                            {
                                for (int c = 0; c < 5; c++)
                                {
                                    arearesult[r, c] = orientations[r, c, rnd];
                                }
                            }
                            for (int r = 0; r < 5; r++)
                            {
                                for (int c = 0; c < 5; c++)
                                {
                                    chosenpieces[r, c, k] = arearesult[r, c];

                                }
                            }
                            bool collision = false;
                            bool placement = false;
                            regularity_ok = false;
                            bool adjacent = false;
                            while (!placement)
                            {
                                collision = false;
                                adjacent = false;
                                int x = random.Next(0, 26);
                                int y = random.Next(0, 16);
                                for (int i = 0; i < 5; i++)
                                {
                                    for (int j = 0; j < 5; j++)
                                    {
                                        if (arearesult[i, j] == 1 && gamescreen[y + i, x + j] != 0)
                                        {
                                            collision = true;
                                            break;
                                        }
                                        if (k == 0) // ilk parca icin komsu kontrolu yapma
                                        {
                                            adjacent = true;
                                            break;
                                        }
                                        if (arearesult[i, j] == 1)
                                        {
                                            if (y + i - 1 >= 0 && gamescreen[y + i - 1, x + j] != 0) adjacent = true;
                                            else if (x + j - 1 >= 0 && gamescreen[y + i, x + j - 1] != 0) adjacent = true;
                                            else if (y + i + 1 < 20 && gamescreen[y + i + 1, x + j] != 0) adjacent = true;
                                            else if (x + j + 1 < 30 && gamescreen[y + i, x + j + 1] != 0) adjacent = true;

                                        }

                                    }
                                    if (collision)
                                        break;
                                }
                                if (collision || !adjacent)
                                {
                                    placement = false;
                                }
                                else
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        for (int j = 0; j < 5; j++)
                                        {
                                            if (arearesult[i, j] == 1)
                                                gamescreen[y + i, x + j] = k + 1;
                                        }
                                    }
                                    placement = true;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("regularity giriniz.");
                Console.Write("Min Regularity : ");
                double minReg = Convert.ToDouble(Console.ReadLine());
                Console.Write("Max Regularity : ");
                double maxReg = Convert.ToDouble(Console.ReadLine());

                int counter = 0;
                for (int k = 0; k < 20; k++)
                {
                    for (int l = 0; l < 30; l++)
                    {
                        if (gamescreen[k, l] != 0)
                        {
                            if (k - 1 >= 0 && gamescreen[k - 1, l] == 0) counter++;
                            else if (k == 0) counter++;
                            if (l - 1 >= 0 && gamescreen[k, l - 1] == 0) counter++;
                            else if (l == 0) counter++;
                            if (k + 1 < 20 && gamescreen[k + 1, l] == 0) counter++;
                            else if (k + 1 == 20) counter++;
                            if (l + 1 < 30 && gamescreen[k, l + 1] == 0) counter++;
                            else if (l + 1 == 30) counter++;
                        }
                    }
                }
                double regularity = sum / Math.Pow((double)counter / 4.0, 2);
                if (minReg <= regularity && regularity <= maxReg)
                {
                    regularity_ok = true;
                    roundScore = sum * Math.Pow(4 * regularity, 4);
                    Score += roundScore;
                }
                int try1 = 10000;
                int try0 = 0;
                while (!regularity_ok && try0 < try1)
                {
                    try0++;
                    gamescreen = Placement(chosenpieces, piecenumber);
                    counter = 0;
                    for (int i = 0; i < 20; i++)
                    {
                        for (int j = 0; j < 30; j++)
                            if (gamescreen[i, j] != 0)
                            {
                                if (i == 0 || gamescreen[i - 1, j] == 0) counter++;
                                if (j == 0 || gamescreen[i, j - 1] == 0) counter++;
                                if (i == 19 || gamescreen[i + 1, j] == 0) counter++;
                                if (j == 29 || gamescreen[i, j + 1] == 0) counter++;
                            }
                    }
                    regularity = sum / Math.Pow(counter / 4.0, 2);
                    if (minReg <= regularity && regularity <= maxReg)
                    {
                        regularity_ok = true;
                        roundScore = sum * Math.Pow(4 * regularity, 4);
                        Score += roundScore;
                    }
                }
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        if (gamescreen[i, j] == 0)
                            baseScreen[i, j] = '.';
                        else
                            baseScreen[i, j] = (char)('A' + gamescreen[i, j] - 1);
                    }
                }
                Console.Clear();
                DrawScreen(baseScreen, placedScreen);

                Console.SetCursorPosition(0, 21);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Original puzzle shown. Press any key to continue...");
                Console.ResetColor();
                Console.ReadKey(true);
                Console.Clear();

                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        baseScreen[i, j] = (gamescreen[i, j] == 0) ? '.' : 'X';
                    }
                }
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        placedScreen[i, j] = ' ';
                    }
                }
                for (int i = 0; i < piecenumber; i++)
                {
                    piecePlaced[i] = false;
                }
                ConsoleKeyInfo key;
                bool gameFinished = false;
                while (!gameFinished)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        for (int j = 0; j < 30; j++)
                        {
                            placedScreen[i, j] = ' ';
                        }
                    }
                    if (selectedPiece != -1)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                if (currentShape[i, j] == 1)
                                {
                                    int y = pieceY + i;
                                    int x = pieceX + j;

                                    if (y >= 0 && y < 20 && x >= 0 && x < 30)
                                        placedScreen[y, x] = (char)('A' + selectedPiece);
                                }
                            }
                        }
                    }
                    DrawScreen(baseScreen, placedScreen);
                    #region print pieces at right side
                    int x2 = 65;  // right
                    int y2 = 2;
                    int x3 = x2;
                    int y3 = y2;


                    for (int i = 0; i < piecenumber; i++)
                    {
                        int[,] p = CopyPiece(chosenpieces, i);
                        PrintPiece(harfler[i], p, x2, y2);

                        x2 += 11;

                        if ((i + 1) % 5 == 0)
                        {
                            x2 = x3;
                            y2 += 6;
                        }
                    }
                    int information = y3 + 13;
                    Console.SetCursorPosition(65, information);
                    Console.WriteLine("Round: " + round);

                    Console.SetCursorPosition(65, information + 2);
                    Console.WriteLine("Regularity: " + regularity.ToString("0.0000"));

                    Console.SetCursorPosition(65, information + 4);
                    Console.WriteLine("Round Score: " + roundScore.ToString("0.00"));
                    #endregion
                    key = Console.ReadKey(true);
                    if ((key.KeyChar >= 'a' && key.KeyChar < 'a' + piecenumber) || (key.KeyChar >= 'A' && key.KeyChar < 'A' + piecenumber))
                    {
                        int idx;

                        if (key.KeyChar >= 'a')
                        {
                            idx = key.KeyChar - 'a';
                        }
                        else
                        {
                            idx = key.KeyChar - 'A';
                        }
                        if (!piecePlaced[idx])
                        {
                            selectedPiece = idx;
                            pieceX = 0;
                            pieceY = 0;
                            for (int r = 0; r < 5; r++)
                            {
                                for (int c = 0; c < 5; c++)
                                {
                                    currentShape[r, c] = chosenpieces[r, c, idx];
                                }
                            }
                        }
                    }
                    if (selectedPiece != -1)
                    {
                        if (key.Key == ConsoleKey.RightArrow)
                        {
                            int shapeWidth = GetShapeWidth(currentShape);
                            if (pieceX + shapeWidth < 30)
                                pieceX++;
                        }
                        if (key.Key == ConsoleKey.LeftArrow && pieceX > 0) pieceX--;
                        if (key.Key == ConsoleKey.UpArrow && pieceY > 0) pieceY--;
                        if (key.Key == ConsoleKey.DownArrow)
                        {
                            int shapeHeight = GetShapeHeight(currentShape);
                            if (pieceY + shapeHeight < 20)
                                pieceY++;
                        }
                        if (key.Key == ConsoleKey.R)
                        {
                            currentShape = RotatePiece(currentShape);
                        }
                        if (key.Key == ConsoleKey.M)
                        {
                            currentShape = ReversePiece(currentShape);
                        }
                        if (key.Key == ConsoleKey.Enter)
                        {
                            if (CanPlace(currentShape, pieceX, pieceY, baseScreen))
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    for (int j = 0; j < 5; j++)
                                    {
                                        if (currentShape[i, j] == 1)
                                        {
                                            int y = pieceY + i;
                                            int x = pieceX + j;
                                            baseScreen[y, x] = (char)('A' + selectedPiece);
                                        }
                                    }
                                }
                                piecePlaced[selectedPiece] = true;
                                selectedPiece = -1;
                                // uyarıyı temizle
                                Console.SetCursorPosition(0, 22);
                                Console.Write(new string(' ', 50));
                            }
                            else
                            {
                                ShowWarning("You cannot enter the piece here!");
                                break;
                            }
                        }
                    }
                    bool allPlaced = true;
                    for (int i = 0; i < piecenumber; i++)
                    {
                        if (!piecePlaced[i]) allPlaced = false;
                    }
                    if (allPlaced)
                    {
                        gameFinished = true;
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Round: " + round);
                Console.WriteLine("Perimeter: " + counter);
                Console.WriteLine("Regularity: " + regularity.ToString("0.0000"));
                Console.WriteLine("Round Score: " + roundScore.ToString("0.00"));
                Console.WriteLine("Total Score: " + Score.ToString("0.00"));
                Console.WriteLine();
                Console.Write("Another round?(yes or no): ");
                string answer = Console.ReadLine().ToLower();
                if (answer != "yes") break;
                round++;
            }
            Console.WriteLine("Final Score: " + Score.ToString("0.00"));
            Console.ReadKey();
        }
        #endregion
    }
}
githuba eklemeli miyim
