using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping_Cart_2.Constants;
using Shopping_Cart_2.Data;
using Shopping_Cart_2.Models;
using Shopping_Cart_2.Services;

namespace Shopping_Cart_2.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class AdminOperationsController : Controller
    {
        private readonly IUserOrderService _userOrderService;
        private readonly IManageItemService _manageItemService;
        private readonly ICategoryService _categoryService;
        public AdminOperationsController(IUserOrderService userOrderService, IManageItemService manageItemService, ICategoryService categoryService)
        {
            _userOrderService = userOrderService;
            _manageItemService = manageItemService;
            _categoryService = categoryService;

        }


        public async Task<IActionResult> AllOrders()
        {
            var orders = await _userOrderService.AllOrders();
            return View(orders);
        }

        
        [HttpGet]
        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            var order = await _userOrderService.GetOrderById(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with id:{orderId} does not found.");
            }
            var orderStatusList = _userOrderService.GetSelectLists();
            var data = new UpdateOrderStatusModel
            {
                OrderId = orderId,
                OrderStatusId = order.OrderStatusId,
                OrderStatusList = orderStatusList
            };
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusModel data)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    data.OrderStatusList =  _userOrderService.GetSelectLists();
                    return View(data);
                }
                await _userOrderService.ChangeOrderStatus(data);
                TempData["msg"] = "Updated successfully";
            }
            catch
            {   // catch exception here
                TempData["msg"] = "Something went wrong";
            }
            return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
        }

        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _userOrderService.TogglePaymentStatus(orderId);
            }
            catch (Exception ex)
            {
                // log exception here
            }
            return RedirectToAction(nameof(AllOrders));
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task <IActionResult> ToggleApprovementStatus(int ItemId)
        {
            try
            {
                await _manageItemService.ToggleApprovementStatus(ItemId);
            }
            catch (Exception ex)
            {
                // log exception here
            }
            return RedirectToAction(nameof(GetAllItems));
             
        }

        public async Task <IActionResult> GetAllItems()
        {
            var items = await _manageItemService.GetAllItems();
            return View(items);
        }
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return View(categories);
        }

        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddCategory(category);
                return RedirectToAction(nameof(GetAllCategories));
            }
            return View(category);
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateCategory(category);
                return RedirectToAction(nameof(GetAllCategories));
            }
            return View(category);
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("DeleteCategory")]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            await _categoryService.DeleteCategory(id);
            return RedirectToAction(nameof(GetAllCategories));
        }



    }
}
