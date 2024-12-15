import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddProductRfComponent } from './add-product-rf.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';

describe('AddProductRfComponent', () => {
  let component: AddProductRfComponent;
  let fixture: ComponentFixture<AddProductRfComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, ReactiveFormsModule,  RouterTestingModule],
      declarations: [AddProductRfComponent]
    });
    fixture = TestBed.createComponent(AddProductRfComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
