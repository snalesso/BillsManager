Imports System.ComponentModel

Namespace Models

    Public Class Fattura
        Implements IDataErrorInfo

        Public ReadOnly Property [Error] As String Implements System.ComponentModel.IDataErrorInfo.Error
            Get
                Return TryCast(Me, IDataErrorInfo).Error
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal columnName As String) As String Implements System.ComponentModel.IDataErrorInfo.Item
            Get
                Dim [error] As String = Nothing

                [error] = (TryCast(Me, IDataErrorInfo))(columnName)

                Return [error]

            End Get
        End Property

        Public ReadOnly Property HasErrors As Boolean
            Get
                For Each [property] As String In ValidatedProperties
                    If GetValidationError([property]) IsNot Nothing Then
                        Return True
                    End If
                Next [property]
                Return False
            End Get
        End Property

        'Un array di nomi di proprietà ammesse alla verifica
        'del loro contenuto
        Private Shared ReadOnly ValidatedProperties() As String = {"Importo", "IVA", "Numero"}

        'Determina la proprietà da verificare
        'e invoca il metodo appropriato
        Private Function GetValidationError(ByVal propertyName As String) As String

            If Array.IndexOf(ValidatedProperties, propertyName) < 0 Then
                Return Nothing
            End If

            Dim [error] As String = Nothing

            Select Case propertyName

                Case "Importo"
                    [error] = Me.ValidateImporto

                    'Case "IVA"
                    '    [error] = Me.ValidateIVA

                Case "Numero"
                    [error] = Me.ValidateNumero

                Case Else
                    Debug.Fail("La proprietà specificata non è verificabile: " & propertyName)

            End Select

            Return [error]

        End Function

        Private Function ValidateImporto() As String
            If String.IsNullOrEmpty(Me.Importo) Then
                Return "L'importo non può essere vuoto"
            Else
                Return Nothing
            End If
        End Function

        'Private Function ValidateIVA() As String
        '    If String.IsNullOrEmpty(Me.IVA) Then
        '        Return "L'IVA non può essere vuota"
        '    Else
        '        Return Nothing
        '    End If
        'End Function

        Private Function ValidateNumero() As String
            If String.IsNullOrEmpty(Me.Numero) Then
                Return "Il numero della fattura non può essere vuoto"
            Else
                Return Nothing
            End If
        End Function

    End Class

End Namespace