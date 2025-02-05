﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SideXC.WebUI.Data;
using SideXC.WebUI.Models.Inventory;

namespace SideXC.WebUI.Controllers.Inventory
{
    public class PaymentMethodsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public PaymentMethodsController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Index view
        /// </summary>
        /// <returns></returns>
        [Authorization]
        public async Task<IActionResult> Index()
        {
            return View(await _context.PaymentMethods.ToListAsync());
        }
        /// <summary>
        /// Create view
        /// </summary>
        /// <returns></returns>
        [Authorization]
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Create post
        /// </summary>
        /// <param name="paymentMethod"></param>
        /// <returns></returns>
        [Authorization]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,NumberOfDays,Active,Created,Modified")] PaymentMethod paymentMethod)
        {
            if (PaymentMethodExists(paymentMethod.Description))
            {
                ModelState.AddModelError("Error", "Ya existe un metódo de pago con esa descripción.");
            }
            if (ModelState.IsValid)
            {
                paymentMethod.Active = true;
                paymentMethod.Created = DateTime.Now;
                paymentMethod.CreatedBy = UserLogged;
                paymentMethod.Modified = DateTime.Now;
                paymentMethod.ModifiedBy = UserLogged;
                _context.Add(paymentMethod);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paymentMethod);
        }
        /// <summary>
        /// Edit view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>        
        [Authorization]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { return NotFound(); }
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null) { return NotFound(); }
            return View(paymentMethod);
        }
        /// <summary>
        /// Edit post
        /// </summary>
        /// <param name="id"></param>
        /// <param name="paymentMethod"></param>
        /// <returns></returns>        
        [Authorization]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,NumberOfDays,Active,Created,Modified")] PaymentMethod paymentMethod, IFormCollection collection)
        {
            var description = collection["hddDescription"].ToString();
            if (id != paymentMethod.Id) { return NotFound(); }
            if(paymentMethod.Description != description)
            {
                if (PaymentMethodExists(paymentMethod.Description))
                {
                    ModelState.AddModelError("Error", "Ya existe un metódo de pago con esa descripción.");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    paymentMethod.Modified = DateTime.Now;
                    paymentMethod.ModifiedBy = UserLogged;
                    _context.Update(paymentMethod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentMethodExists(paymentMethod.Description)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(paymentMethod);
        }
        /// <summary>
        /// Validation
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private bool PaymentMethodExists(string description)
        {
            return _context.PaymentMethods.Any(e => e.Description == description);
        }
    }
}
