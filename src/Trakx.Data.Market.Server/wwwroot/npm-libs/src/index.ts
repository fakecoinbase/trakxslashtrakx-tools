//import * as signalR from '@microsoft/signalr';

//const symbolMessage: HTMLDivElement | null = document.querySelector('#symbol');
//const priceMessage: HTMLDivElement | null = document.querySelector('#price');
//const username = new Date().getTime();

//const connection = new signalR.HubConnectionBuilder().withUrl('/hubs/nav').build();

//connection.on('UpdateNav', (symbol: string, price: number) => {
//    const m = document.createElement('div');

//    if (symbolMessage) symbolMessage.innerText = symbol;
//    if (priceMessage) priceMessage.innerText = price.toString();
//});

//connection.start().catch(err => document.write(err));

