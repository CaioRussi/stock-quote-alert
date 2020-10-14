# stock-quote-alert

Aplicação worker service para monitoramento de ativos

## Dependências

[.Net core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

## Gerar o executável

```sh
$ cd stock-quote-alert
$ dotnet restore
$ dotnet publish -c Release
```

Executável gerado no path \bin\Release\netcoreapp3.1\publish

## Configuração

As configurações da API e do servidor de SMTP se encontram no arquivo appsettings.json no mesmo caminho do executável gerado.

## Executar

Parâmetros:

* ativo: o ativo a ser monitorado
* saleValue: valor sugerido para a venda
* purchaseValue: valor sugerido para a compra

```sh
$ stock-quote-alert.exe --ativo=petr4 --saleValue=15.0 --purchaseValue=10.0
```
