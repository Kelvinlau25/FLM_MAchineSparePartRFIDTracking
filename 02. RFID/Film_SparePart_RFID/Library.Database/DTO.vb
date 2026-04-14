''' <summary>
''' Passing Data between Library and Interface
''' </summary>
Public Class DTO

    Private _table1 As DataTable
    Private _error1 As Boolean
    Private _errorString1 As String
    Private _str1 As String
    Private _int1 As Integer

    Public Property Table As DataTable
        Get
            Return _table1
        End Get
        Set(value As DataTable)
            _table1 = value
        End Set
    End Property

    ''' <summary>
    ''' Set Error = true if hit error
    ''' </summary>
    ''' <returns></returns>
    Public Property [Error] As Boolean
        Get
            Return _error1
        End Get
        Set(value As Boolean)
            _error1 = value
        End Set
    End Property

    ''' <summary>
    ''' Pass Error Message 
    ''' </summary>
    ''' <returns></returns>
    Public Property ErrorMessage As String
        Get
            Return _errorString1
        End Get
        Set(value As String)
            _errorString1 = value
        End Set
    End Property

    Public Property Str As String
        Get
            Return _str1
        End Get
        Set(value As String)
            _str1 = value
        End Set
    End Property

    Public Property Int As Integer
        Get
            Return _int1
        End Get
        Set(value As Integer)
            _int1 = value
        End Set
    End Property

End Class
