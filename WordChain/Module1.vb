Imports System.IO

Module Module1

    Structure Score
        Dim PlayerName As String
        Dim OpponentName As String
        Dim Score As Integer
    End Structure

    Sub Main()
        Dim ScoreStructureTotal As Integer = 3
        'Used with the NumberOfScores to know how many socres there are
        Dim ToLoad As Integer = NumberOfScores(ScoreStructureTotal)
        Dim UserChoice As Char
        Dim Player1 As String = ""
        Dim Player2 As String = ""
        Do
            Console.WriteLine("Welcome to the Word Chain Game")
            Console.WriteLine("")
            Console.WriteLine("Please select one of the following options")
            Console.WriteLine("1. Play the Game")
            Console.WriteLine("2. Read the Rules")
            Console.WriteLine("3. Scores")
            Console.WriteLine("4. Exit")
            UserChoice = GetUserChoice()
            Select Case UserChoice
                Case "1"
                    'Scores/ToLoad are here due to them needing to be added to each game (because of a newscore beeing added)
                    ToLoad = ToLoad + 1
                    Dim Scores(ToLoad) As Score
                    LoadScores(Scores)
                    PlayGame(Player1, Player2, Scores, ToLoad)
                Case "2"
                    Console.Clear()
                    Rules()
                Case "3"
                    Console.Clear()
                    Dim Scores(ToLoad) As Score
                    LoadScores(Scores)
                    ShowRecentScores(Scores, ToLoad)
                    'ShowTopScores(ScoreStructureTotal)
                Case "4"
                    Environment.Exit(0)
                Case Else
                    Console.Clear()
            End Select
        Loop
    End Sub

    'Going to be used in a later version to enable users to it change through a settings menu
    'The change will need to be saved to an extrernal file, and loaded here, to stay after closing
    Function ToShow()
        Dim ToLoadVal As Integer
        FileOpen(1, "ToLoad.txt", OpenMode.Input)
        While Not EOF(1)
            ToShow = LineInput(1)
        End While
        FileClose(1)
        Return ToLoadVal
    End Function

    Function NumberOfScores(ByRef ScoreStructureTotal As Integer)
        Dim Count As Integer = 1
        Dim Number As Integer
        FileOpen(1, "RecentScores.txt", OpenMode.Input)
        'Counts how many lines are in the file
        While Not EOF(1)
            LineInput(1)
            Count = Count + 1
        End While
        FileClose(1)
        'diivids the lines by the amount of varibles in a score to see how many socres there are
        Number = Count / ScoreStructureTotal
        Return Number
    End Function

    Function GetUserChoice() As Char
        Dim UserChoice As Char
        UserChoice = Console.ReadLine
        Console.WriteLine()
        Return UserChoice
    End Function

    Sub LoadScores(ByVal Scores() As Score)
        Dim Count As Integer = 1
        FileOpen(1, "RecentScores.txt", OpenMode.Input)
        While Not EOF(1)
            Scores(Count).PlayerName = LineInput(1)
            Scores(Count).OpponentName = LineInput(1)
            Scores(Count).Score = LineInput(1)
            Count = Count + 1
        End While
        FileClose(1)
    End Sub

    Sub ShowRecentScores(ByVal Scores() As Score, ByVal ToLoad As Integer)
        Dim Count As Integer
        Console.WriteLine("Scores:")
        Console.WriteLine()
        For Count = 1 To ToLoad
            Console.WriteLine(Scores(Count).PlayerName & " won against " & Scores(Count).OpponentName & " with " & Scores(Count).Score & " words played.")
        Next
        Console.WriteLine()
        Console.WriteLine("Press the Enter key to return to the main menu")
        Console.WriteLine()
        Console.ReadLine()
        Console.Clear()
    End Sub

    'To be added later into a second score option through the menu.
    Sub ShowTopScores(ByRef ScoreStructureTotal As Integer)
        'Basic code to create the array, with a size determined by the ToLoad Method I have created.
        'Then to inset the scores from an external file via a method into the "Scores" array.
        Dim ToLoad As Integer = NumberOfScores(ScoreStructureTotal)
        Dim Scores(ToLoad) As Score
        LoadScores(Scores)
        Dim Count As Integer
        Dim Last3 As Integer = ToLoad - 3
        Console.WriteLine("Recent scores:")
        Console.WriteLine()
        For Count = ToLoad - Last3 To ToLoad
            Console.WriteLine(Scores(Count).PlayerName & " won against " & Scores(Count).OpponentName & " with " & Scores(Count).Score & " words played.")
        Next
        Console.WriteLine()
        Console.WriteLine("Press the Enter key to return to the main menu")
        Console.WriteLine()
        Console.ReadLine()
        Console.Clear()
    End Sub

    Sub PlayGame(ByVal Player1 As String, ByVal Player2 As String, ByVal Scores() As Score, ByVal ToLoad As Integer)
        Dim WordsUsed As New System.Collections.ArrayList()
        Dim NumberOfGoes As Integer = 1
        Dim WordInput As String
        Dim LastWord As String = ""
        'EOG = End of Game
        Dim EOG As Boolean = False

        Console.Clear()
        Console.WriteLine("Player 1's Name:")
        Player1 = Console.ReadLine()
        Console.WriteLine("Player 2's Name:")
        Player2 = Console.ReadLine()
        Console.WriteLine("")
        Console.WriteLine("THE GAME HAS BEGUN!")

        'Game Loop Start
        Do
            Dim WordExists As Boolean
            Dim Rule1Broken As Boolean
            Dim Rule2Broken As Boolean
            Dim Rule3Broken As Boolean

            'Whos Turn Code
            If WhosTurn(NumberOfGoes) = False Then
                Console.WriteLine("{0}'s Turn:", Player1)
            Else
                Console.WriteLine("{0}'s Turn:", Player2)
            End If

            'Retrives the users input
            WordInput = Console.ReadLine().ToUpper

            'Cannot return a blank value
            If WordInput = "" Then
                Rule1Broken = True
            End If

            'Check if the word is in the dictionary/wordlist (Rule 1)
            WordExists = CheckWord(WordInput)
            If WordExists = True Then
                Rule1Broken = False
            Else
                Rule1Broken = True
            End If

            'Failsafe for first turn
            If NumberOfGoes > 1 Then
                'Checks if the input word starts with the last words last char' (Rule 2)
                If ReturnFirstChar(WordInput) = ReturnLastChar(LastWord) Then
                    Rule2Broken = False
                Else
                    Rule2Broken = True
                End If
            End If

            'Checks if the word has been used before (Rule 3)
            If WordsUsed.Contains(WordInput) Then
                Rule3Broken = True
            Else
                Rule3Broken = False
            End If

            'Checks if any rules were broken
            If Rule1Broken = True Or Rule2Broken = True Or Rule3Broken = True Then
                EOG = True
                If Rule1Broken = True Then
                    Console.WriteLine("That word does not exist.")
                End If
                If Rule2Broken = True Then
                    Console.WriteLine("That word does not start with the first letter of the last word.")
                End If
                If Rule3Broken = True Then
                    Console.WriteLine("This word has already been used.")
                End If
            End If

            If EOG = False Then
                'Next turn setup
                LastWord = WordInput
                NumberOfGoes = NumberOfGoes + 1
                WordsUsed.Add(WordInput)
            End If

        Loop Until EOG = True
        'Game's finished

        'Print
        If WhosTurn(NumberOfGoes) = False Then
            Console.WriteLine("{0} Won, after {1} words...", Player2, NumberOfGoes - 1)
            Scores(ToLoad).PlayerName = Player2
            Scores(ToLoad).OpponentName = Player1
            Scores(ToLoad).Score = NumberOfGoes - 1
        Else
            Console.WriteLine("{0} Won, after {1} words...", Player1, NumberOfGoes - 1)
            Scores(ToLoad).PlayerName = Player1
            Scores(ToLoad).OpponentName = Player2
            Scores(ToLoad).Score = NumberOfGoes - 1
        End If

        'Appends the score to the file "RecentScores"
        WriteToScoresFile(Scores, ToLoad)
        'Ending options
        Console.WriteLine("Click Enter to go back to the main menu.")
        Console.ReadLine()
        Console.Clear()
    End Sub

    Sub WriteToScoresFile(ByVal Scores() As Score, ByRef ToLoad As Integer)
        Dim append As Boolean = True
        Using writer As System.IO.StreamWriter = New System.IO.StreamWriter("RecentScores.txt", append)
            writer.WriteLine(Scores(ToLoad).PlayerName)
            writer.WriteLine(Scores(ToLoad).OpponentName)
            writer.WriteLine(Scores(ToLoad).Score)
        End Using
    End Sub

    Sub Rules()
        Console.WriteLine("Rules")
        Console.WriteLine()
        Console.WriteLine("1. You must type a real word from the dictionary.")
        Console.WriteLine("2. This word must begin with the last letter of the previous word.")
        Console.WriteLine("2a. Unless this is the first word, then it can begin in any letter.")
        Console.WriteLine("3. You can not use a word that has been used before.")
        Console.WriteLine("If you break any of these rules, you will lose the game.")
        Console.WriteLine()
        Console.WriteLine("Press the Enter key to return to the main menu")
        Console.ReadLine()
        Console.Clear()
    End Sub

    Function ReturnFirstChar(ByRef WordInput As String) As Char
        Dim Character As Char
        Character = WordInput.Substring(0, 1)
        Return Character
    End Function

    Function ReturnLastChar(ByRef LastWord As String) As Char
        Dim Character As Char
        Character = LastWord.Substring(LastWord.Length - 1)
        Return Character
    End Function

    Function CheckWord(ByVal WordInput As String) As Boolean
        Dim WordIsReal As Boolean = False
        Dim myReader As StreamReader
        myReader = File.OpenText("WordList.txt")
        Dim line As String = ""

        While Not IsNothing(line)
            line = myReader.ReadLine()
            If Not IsNothing(line) Then
                'It will go through each word and if the users inputted word is in the wordlist, it will set the varible true
                If WordInput = line Then
                    WordIsReal = True
                End If
            End If
        End While
        Return WordIsReal
    End Function

    Function WhosTurn(ByVal NumberOfGoes As Integer) As Boolean
        Dim PlayerTwosGo As Boolean
        If NumberOfGoes Mod 2 = 0 Then
            PlayerTwosGo = True
        Else
            PlayerTwosGo = False
        End If
        Return PlayerTwosGo
    End Function
End Module
