﻿using Library.Entities.Models;
using Library.Entities.Repositories;
using Library_Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Library.Web.Areas.Librarian.Controllers
{
    [Area(StaticData.LibrarianRole)]
    [Authorize(Roles = StaticData.LibrarianRole)]
    public class CheckoutController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CheckoutController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var confirmedByUserCheckouts = await unitOfWork.CheckoutRepository.GetAllAsync(c => c.Status == StaticData.ConfirmedByUser,"Book,ApplicationUser");
            return View(confirmedByUserCheckouts);
        }
        [HttpGet]
        public async Task<IActionResult> Approve(int id)
        {
            var checkout = await unitOfWork.CheckoutRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            var book = await unitOfWork.BookRepository.GetFirstOrDefaultAsync(x => x.Id == checkout.BookId);
            
            if (book.Stock == 0)
            {
                TempData["Delete"] = "Checkout Can't be made due empty stock";
                return RedirectToAction("Index");
            }

            checkout.Status = StaticData.ApprovedByAdmin;
            checkout.DueDate = DateTime.Now.AddDays(StaticData.ReturnDays);
            checkout.CheckoutDate = DateTime.Now;

            book.Stock -= 1; 
            
            await unitOfWork.CompleteAsync();
            TempData["Success"] = "Checkout approved!";


            return RedirectToAction("Index");
        }  
        [HttpGet]
        public async Task<IActionResult> Disapprove(int id)
        {
            var checkout = await unitOfWork.CheckoutRepository.GetFirstOrDefaultAsync(x => x.Id == id);
            
            checkout.Status = StaticData.DisaprrovedByAdmin;
            
            await unitOfWork.CompleteAsync();
            TempData["Success"] = "Checkout Disapproved!";


            return RedirectToAction("Index");
        } 
        
        [HttpGet]
        public async Task<IActionResult> Search(string? username)
        {
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(username))
            {
                TempData["Delete"] = "Empty Input!";
                return RedirectToAction("Index");
            }
            var checkouts = await unitOfWork.CheckoutRepository.GetAllCheckoutsFilterdByUsernnameAsync(username);

            if (checkouts.IsNullOrEmpty())
            {
                TempData["Delete"] = "Not Found!";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", checkouts);
        }
    }
}