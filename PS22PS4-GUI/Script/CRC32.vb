Option Explicit On
Option Strict On

<System.Diagnostics.DebuggerStepThrough()>
Friend Class CRC32
    Private crc32Table() As Integer
    Private Const BUFFER_SIZE As Integer = 1024I

    Friend Function GetCrc32(ByRef stream As System.IO.FileStream) As Integer
        Dim crc32Result As Integer = &HFFFFFFFF

        Dim buffer(BUFFER_SIZE) As Byte
        Dim readSize As Integer = BUFFER_SIZE
        Dim count As Integer = stream.Read(buffer, 0I, readSize)
        Dim i As Integer
        Dim iLookup As Integer

        Do While (count > 0I)
            For i = 0I To count - 1I
                iLookup = (crc32Result And &HFF) Xor buffer(i)
                crc32Result = ((crc32Result And &HFFFFFF00) \ &H100) And &HFFFFFF   ' nasty shr 8 with vb :/
                crc32Result = crc32Result Xor crc32Table(iLookup)
            Next i
            count = stream.Read(buffer, 0I, readSize)
        Loop
        Return Not (crc32Result)
    End Function

    Friend Function GetCrc32String(ByRef stream As System.IO.FileStream) As String
        Return String.Format("{0:X8}", GetCrc32(stream))
    End Function

    Friend Sub New()
        Dim dwPolynomial As Integer = &HEDB88320
        Dim i, j As Integer

        ReDim crc32Table(256I)
        Dim dwCrc As Integer

        For i = 0I To 255I
            dwCrc = i
            For j = 8I To 1I Step -1I
                If (dwCrc And 1I) > 0I Then
                    dwCrc = ((dwCrc And &HFFFFFFFE) \ 2I) And &H7FFFFFFF
                    dwCrc = dwCrc Xor dwPolynomial
                Else
                    dwCrc = ((dwCrc And &HFFFFFFFE) \ 2I) And &H7FFFFFFF
                End If
            Next j
            crc32Table(i) = dwCrc
        Next i
    End Sub
End Class