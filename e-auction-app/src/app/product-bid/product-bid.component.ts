import { Component, OnInit } from '@angular/core';

export interface PeriodicElement {
  name: string;
  bidAmount: string;
  email: string;
  mobile: string;
}

const ELEMENT_DATA: PeriodicElement[] = [
  {bidAmount: '100', name: 'Hydrogen', email: 'abc@gmail.com', mobile: '1234567891'},
  {bidAmount: '200', name: 'Hydrogen', email: 'abc@gmail.com', mobile: '1234567891'},
  {bidAmount: '300', name: 'Hydrogen', email: 'abc@gmail.com', mobile: '1234567891'},
  {bidAmount: '400', name: 'Hydrogen', email: 'abc@gmail.com', mobile: '1234567891'},
  {bidAmount: '500', name: 'Hydrogen', email: 'abc@gmail.com', mobile: '1234567891'},
];
@Component({
  selector: 'app-product-bid',
  templateUrl: './product-bid.component.html',
  styleUrls: ['./product-bid.component.css']
})
export class ProductBidComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

  displayedColumns: string[] = ['bidAmount', 'name', 'email', 'mobile'];
  dataSource = ELEMENT_DATA;

}
