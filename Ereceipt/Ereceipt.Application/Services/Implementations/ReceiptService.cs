﻿using AutoMapper;
using Ereceipt.Application.Extensions;
using Ereceipt.Application.Results;
using Ereceipt.Application.Results.Receipts;
using Ereceipt.Application.Services.Interfaces;
using Ereceipt.Application.ViewModels.Currency;
using Ereceipt.Application.ViewModels.Receipt;
using Ereceipt.Domain.Interfaces;
using Ereceipt.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Ereceipt.Application.Services.Implementations
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepos;
        private readonly IProductService _productService;
        private readonly IJsonConverter _jsonConverter;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IMapper _mapper;
        public ReceiptService(IReceiptRepository ReceiptRepos, IMapper mapper, IProductService productService, ICurrencyRepository currencyRepository, IJsonConverter jsonConverter)
        {
            _receiptRepos = ReceiptRepos;
            _mapper = mapper;
            _productService = productService;
            _currencyRepository = currencyRepository;
            _jsonConverter = jsonConverter;
        }

        public async Task<ReceiptResult> AddReceiptToGroupAsync(ReceiptGroupCreateModel model)
        {
            var Receipt = await _receiptRepos.FindAsTrackingAsync(d => d.Id == model.ReceiptId);
            if (Receipt == null)
                return new ReceiptResult("Receipt not found");
            if (Receipt.UserId != model.UserId)
                return new ReceiptResult("Access Denited");
            if (Receipt.GroupId != null)
                return new ReceiptResult("Already in group");
            Receipt.GroupId = model.GroupId;
            Receipt.LastUpdatedAt = DateTime.UtcNow;
            Receipt.LastUpdatedBy = model.UserId.ToString();
            return new ReceiptResult(_mapper.Map<ReceiptViewModel>(await _receiptRepos.UpdateAsync(Receipt)));
        }

        public async Task<ReceiptResult> CreateReceiptAsync(ReceiptCreateModel model)
        {
            CurrencyViewModel currency;
            if (model.CurrencyId == null)
                currency = _mapper.Map<CurrencyViewModel>(await _currencyRepository.FindAsync(d => d.Code == "UAH"));
            else
            {
                currency = _mapper.Map<CurrencyViewModel>(await _currencyRepository.FindAsync(d => d.Id == model.CurrencyId));
            }
            var receiptToCreate = new Receipt
            {
                ShopName = model.ShopName,
                AddressShop = model.AddressShop,
                IsImportant = model.IsImportant,
                UserId = model.UserId,
                GroupId = model.GroupId,
                Currency = _jsonConverter.GetStringAsJson(currency),
                ReceiptType = ReceiptType.Internal,
                TotalPrice = model.TotalPrice == default ? model.Products.Sum(x=>x.Price) : default,
                Products = _jsonConverter.GetStringAsJson(model.Products),
                CreatedBy = model.UserId.ToString()
            };
            receiptToCreate.SetInitData(model);
            return new ReceiptResult(_mapper.Map<ReceiptViewModel>(await _receiptRepos.CreateAsync(receiptToCreate)));
        }

        public async Task<ReceiptResult> EditReceiptAsync(ReceiptEditModel model)
        {
            var receiptForEdit = await _receiptRepos.FindAsTrackingAsync(d => d.Id == model.Id);
            if (receiptForEdit == null)
                return new ReceiptResult("Receipt not found");
            if (receiptForEdit.UserId != model.UserId)
                return new ReceiptResult("Access Denited");
            if (!receiptForEdit.CanEdit)
                return new ReceiptResult("This receipt can`t be edit");
            if (model.CurrencyId is not null)
            {
                var newCurrency = await _currencyRepository.FindAsync(d => d.Id == model.CurrencyId);
                if (newCurrency is null)
                    return new ReceiptResult("This currency not exist");
                receiptForEdit.Currency = _jsonConverter.GetStringAsJson(newCurrency);
            }
            receiptForEdit.ShopName = model.ShopName;
            receiptForEdit.AddressShop = model.AddressShop;
            receiptForEdit.IsImportant = model.IsImportant;
            receiptForEdit.Products = _jsonConverter.GetStringAsJson(model.Products);
            receiptForEdit.TotalPrice = model.Products != null ? model.Products.Sum(x => x.Price) : receiptForEdit.TotalPrice;
            receiptForEdit.SetUpdateData(model);
            return new ReceiptResult(_mapper.Map<ReceiptViewModel>(await _receiptRepos.UpdateAsync(receiptForEdit)));
        }

        public async Task<ListReceiptResult> GetAllReceiptsAsync(int skip)
        {
            return new ListReceiptResult(_mapper.Map<List<ReceiptViewModel>>(await _receiptRepos.GetAllAsync(20, skip)));
        }

        public async Task<CountResult> GetAllReceiptsCountAsync()
        {
            return new CountResult(await _receiptRepos.CountAsync());
        }

        public async Task<ReceiptResult> GetReceiptAsync(Guid id)
        {
            var receipt = _mapper.Map<ReceiptViewModel>(await _receiptRepos.GetReceiptByIdAsync(id));
            if (receipt != null)
                receipt.CommentsCount = await _receiptRepos.GetCountCommentsByReceiptIdAsync(id);
            return new ReceiptResult(receipt);
        }

        public async Task<ListReceiptResult> GetUserReceiptsByUserIdAsync(int ownerId, int skip)
        {
            return new ListReceiptResult(_mapper.Map<List<ReceiptViewModel>>(await _receiptRepos.GetReceiptsByUserIdAsync(ownerId, skip)));
        }

        public async Task<CountResult> GetUserReceiptsCountAsync(int ownerId)
        {
            return new CountResult(await _receiptRepos.CountAsync(d => d.UserId == ownerId));
        }

        public async Task<ReceiptResult> RemoveReceiptAsync(Guid id, int userId)
        {
            var receiptToDelete = await _receiptRepos.FindAsTrackingAsync(d => d.Id == id);
            if (receiptToDelete == null)
                return new ReceiptResult("Receipt not found");
            if (userId == 0)
            {
                return new ReceiptResult(_mapper.Map<ReceiptViewModel>(await _receiptRepos.RemoveAsync(receiptToDelete)));
            }
            if (receiptToDelete.UserId != userId)
                return null;
            return new ReceiptResult(_mapper.Map<ReceiptViewModel>(await _receiptRepos.RemoveAsync(receiptToDelete)));
        }

        public async Task<ReceiptResult> RemoveReceiptFromGroupAsync(ReceiptGroupCreateModel model)
        {
            var Receipt = await _receiptRepos.FindAsTrackingAsync(d => d.Id == model.ReceiptId);
            if (Receipt == null)
                return new ReceiptResult("Receipt not found");
            if (Receipt.UserId != model.UserId)
                return new ReceiptResult("Access Denited");
            if (Receipt.GroupId != model.GroupId)
                return new ReceiptResult("GroupId not valid");
            Receipt.GroupId = null;
            Receipt.LastUpdatedAt = DateTime.Now;
            Receipt.LastUpdatedBy = model.UserId.ToString();
            return new ReceiptResult(_mapper.Map<ReceiptViewModel>(await _receiptRepos.UpdateAsync(Receipt)));
        }
    }
}