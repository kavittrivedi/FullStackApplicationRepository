import { Component } from '@angular/core';
 
@Component({
  selector: 'app-pipe-example',
  templateUrl: './pipe-example.component.html',
  styleUrls: ['./pipe-example.component.css']
})
export class PipeExampleComponent {
 today:number=Date.now();
 message:string="Hello Angular";
 amount:number=1234.56123;
 percentage:number=0.85;
 user:{name:string;age:number}={name:"prakruti",age:20}
}