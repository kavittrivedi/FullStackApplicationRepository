import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-template-form',
  templateUrl: './template-form.component.html',
  styleUrls: ['./template-form.component.css']
})
export class TemplateFormComponent {
  user = {
    name: '',
    email: '',
    age: null,
    website: '',
    gender: '',
    country: '',
    interests: {
      sports: false,
      music: false,
      travel: false
    }
  };
  countries = ['USA', 'Canada', 'UK'];
  onSubmit(form: NgForm) {
    if (form.valid) {
      console.log('form submitted', form.value);
    }
  }
}
