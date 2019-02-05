using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sameer.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    public class GeneralMvcController<M,VM> : Controller where M:class,ISameerObject,new() where VM:class,new()
    {

        protected virtual string _objectTypeName { get; set; } = typeof(M).Name;

        protected IMapper _mapper;
        protected ILogger _logger;
        protected IDataManager<M>  _dataManager;

        #region Data Messages

        protected virtual string createNewTitleMessage
        {
            get
            {
                return $"Create a new {_objectTypeName}";
            }
        }

        protected virtual string creatingNewDataMessage
        {
            get
            {
                return $"Creating a new {_objectTypeName}";
            }
        }

        protected virtual string creatingNewDataSuccessfullMessage
        {
            get
            {
                return $"{_objectTypeName} created successfully.";
            }
        }

        protected virtual string creatingNewDataFailMessage
        {
            get
            {
                return $"Could not save new {_objectTypeName} to the database !";
            }
        }

        protected virtual string saveErrorMessage
        {
            get
            {
                return $"Save error please try again later !";
            }
        }

        protected virtual string dataNotFoundMessage
        {
            get
            {
                return $"Can't find {_objectTypeName}";
            }
        }

        protected virtual string getDataNotFoundMessage(int id)
        {
            return $"Can't find {_objectTypeName} with id of {id}";
        }

        protected virtual string updatingDataMessage
        {
            get
            {
                return $"Updating {_objectTypeName}";
            }
        }

        protected virtual string getUpdatingDataMessage(int id)
        {
            return $"Updating {_objectTypeName} with id of {id}";
        }

        protected virtual string updatingDataSuccessfullMessage
        {
            get
            {
                return $"{_objectTypeName} updated successfully.";
            }
        }

        protected virtual string updatingDataFailMessage
        {
            get
            {
                return $"Could not save {_objectTypeName} to the database !";
            }
        }

        protected virtual string deletingDataMessage
        {
            get
            {
                return $"Deleting {_objectTypeName}";
            }
        }

        protected virtual string getDeletingDataMessage(int id)
        {
            return $"Deleting {_objectTypeName} with id of {id}";
        }

        protected virtual string deletingDataSuccessfullMessage
        {
            get
            {
                return $"{_objectTypeName} deleted successfully.";
            }
        }

        protected virtual string deletingDataFailMessage
        {
            get
            {
                return $"Could not delete {_objectTypeName} from the database !";
            }
        }

        #endregion

        public GeneralMvcController(IDataManager<M> dataManager
            , IMapper mapper, ILogger<GeneralMvcController<M, VM>> logger)
        {
            _dataManager = dataManager;
            _mapper = mapper;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public virtual IActionResult Index()
        {
            ViewData["StatusMessage"] = this.StatusMessage;
            return View();
        }

        protected virtual async Task<List<VM>> fillItemsMissingData(List<VM> resultData)
        {
            return await Task.FromResult(resultData);
        }

        protected virtual async Task<VM> fillItemMissingData(VM dataItem)
        {
            var resultDataItem = await fillItemsMissingData(new List<VM>() { dataItem });
            return resultDataItem.First();
        }

        [HttpGet]
        public virtual async Task<IActionResult> IndexAsync()
        {
            try
            {
                ViewData["StatusMessage"] = this.StatusMessage;
                var allDataList = await _dataManager.GetAllDataList();
                var resultDataModelList = await fillItemsMissingData(_mapper.Map<List<VM>>(allDataList));
                return View(resultDataModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAllDataJson()
        {
            var allDataList = await _dataManager.GetAllDataList();
            var resultDataModelList = await fillItemsMissingData(_mapper.Map<List<VM>>(allDataList));
            return Json(resultDataModelList);
        }

        //protected async Task<IActionResult> VerifyData(string fieldName,string fieldValue, int? id = null
        //    ,string errorMessage=null,string exceptionMessage=null)
        //{
        //    try
        //    {
        //        var dataByFieldName = await _dataManager.GetAllDataList(fieldName, fieldValue);
        //        if (dataByFieldName != null && dataByFieldName.Any() && dataByFieldName.FirstOrDefault().Id != id)
        //        {
        //            return Json(errorMessage);
        //        }
        //        return Json(true);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(exceptionMessage);
        //    }
        //}


        public virtual async Task<IActionResult> GetAllDataJsonForKendo([DataSourceRequest] DataSourceRequest request)
        {
            var allDataList = await _dataManager.GetAllDataList();
            var resultDataModelList = await fillItemsMissingData(_mapper.Map<List<VM>>(allDataList));
            return Json(resultDataModelList.ToDataSourceResult(request));
        }

        public virtual async Task<IActionResult> GetAllDataByFilterJsonForKendo([DataSourceRequest] DataSourceRequest request,string filterField,int filterValue)
        {
            var allDataList = await _dataManager.GetAllDataList(filterField,filterValue);
            var resultDataModelList = await fillItemsMissingData(_mapper.Map<List<VM>>(allDataList));
            return Json(resultDataModelList.ToDataSourceResult(request));
        }

        protected virtual async Task<M> getDataById(int id)
        {
            try
            {
                var resultData = await _dataManager.GetDataById(id);
                return await getDataByIdResult(resultData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected virtual async Task<M> getDataByIdResult(M resultData)
        {
            return await Task.FromResult(resultData);
        }

        protected virtual async Task<VM> getDataViewModelById(int id)
        {
            try
            {
                var currentData =  await getDataById(id);

                if(currentData==null)
                {
                    return null;
                }

                var result = await fillItemMissingData(_mapper.Map<VM>(currentData));
                return await getDataViewModelByIdResult(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected virtual async Task<VM> getDataViewModelByIdResult(VM resultData)
        {
            return await Task.FromResult(resultData);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Details(int id)
        {
            try
            {
                ViewData["StatusMessage"] = this.StatusMessage;

                var result = await getDataViewModelById(id);
                
                if (result == null)
                {
                    return NotFound();
                }
                
                return View(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected virtual async Task<VM> createObject()
        {
            return await Task.FromResult(new VM());
        }

        protected virtual IActionResult createView(VM newData)
        {
            return View("Create", newData);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Create()
        {
            try
            {
                ViewData["StatusMessage"] = this.StatusMessage;
                this.StatusMessage = "Cancel Save";
                ViewBag.Title = createNewTitleMessage;
                return createView(await createObject());
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation(creatingNewDataMessage);

                    var validationResults = await checkNewData(model);
                    if (validationResults.Any())
                    {
                        foreach (var vr in validationResults)
                        {
                            if (vr.MemberNames.Count() < 1)
                            {
                                _logger.LogWarning($"validation exception while saving new {_objectTypeName}  error : {vr.ErrorMessage}");
                                ModelState.AddModelError("", vr.ErrorMessage);
                            }
                            else
                            {
                                foreach (var mn in vr.MemberNames)
                                {
                                    _logger.LogWarning($"validation exception while saving new {_objectTypeName} :member name : {mn} error : {vr.ErrorMessage}");
                                    ModelState.AddModelError(mn, vr.ErrorMessage);
                                }
                            }
                        }
                    }
                    else
                    {
                        var newData = _mapper.Map<M>(model);

                        using (_dataManager)
                        {

                            var saveResult = await _dataManager.InsertNewDataItem(newData);

                            if (saveResult.Status == RepositoryActionStatus.Created)
                            {
                                _logger.LogInformation(creatingNewDataSuccessfullMessage);
                                this.StatusMessage = "Save Succeeded";
                                return await createSucceededResult(saveResult.Entity);
                            }
                            else
                            {
                                _logger.LogWarning(creatingNewDataFailMessage);
                                ModelState.AddModelError(string.Empty, saveErrorMessage);
                            }
                        }
                    }
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning($"validation exception while saving new {_objectTypeName} : {ex.ValidationResult.ErrorMessage}");
                    ModelState.AddModelError(ex.ValidationResult.MemberNames.FirstOrDefault()??"", ex.ValidationResult.ErrorMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Throw exception while save new {_objectTypeName} : {ex}");
                    ModelState.AddModelError("", saveErrorMessage);
                }
            }

            this.StatusMessage = "Cancel Save";
            ViewData["StatusMessage"] = "Error - " + ModelState.Where(m => m.Value.Errors.Count() > 0).FirstOrDefault().Value.Errors.FirstOrDefault().ErrorMessage;
            return createView(model);
        }

        protected virtual async Task<List<ValidationResult>> checkNewData(VM newData)
        {
            return await Task.FromResult(new List<ValidationResult>());
        }

        protected virtual async Task<IActionResult> createSucceededResult(M newData)
        {
            return RedirectToAction(nameof(Details), new { id = newData.Id });
        }

        protected virtual IActionResult editView(VM currentData)
        {
            return View("Edit", currentData);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var currentData = await _dataManager.GetDataById(id);

                if (currentData == null)
                {
                    return NotFound();
                }

                return editView(_mapper.Map<VM>(currentData));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation(getUpdatingDataMessage(id));

                    using (_dataManager)
                    {
                        var currentData = await _dataManager.GetDataById(id);
                        if (currentData == null)
                        {
                            _logger.LogWarning(getDataNotFoundMessage(id));
                            return NotFound();
                        }

                        var validationResults = await checkEditData(currentData,model);
                        if (validationResults.Any())
                        {
                            foreach (var vr in validationResults)
                            {
                                foreach (var mn in vr.MemberNames)
                                {
                                    _logger.LogWarning($"validation exception while updating {_objectTypeName} :member name : {mn} error : {vr.ErrorMessage}");
                                    ModelState.AddModelError(mn, vr.ErrorMessage);
                                }
                            }
                        }
                        else
                        {
                            _mapper.Map(model, currentData);

                            var updateResult = await _dataManager.UpdateDataItem(currentData);
                            if (updateResult.Status == RepositoryActionStatus.Updated)
                            {
                                _logger.LogInformation(updatingDataSuccessfullMessage);
                                return editSucceededResult(updateResult.Entity);
                            }
                            else
                            {
                                _logger.LogWarning(updatingDataFailMessage);
                                ModelState.AddModelError(string.Empty, saveErrorMessage);
                            }
                        }                       
                    }
                }
                catch (ValidationException ex)
                {
                    _logger.LogWarning($"validation exception while edit {_objectTypeName} : {ex.ValidationResult.ErrorMessage}");
                    ModelState.AddModelError(ex.ValidationResult.MemberNames.FirstOrDefault() ?? "", ex.ValidationResult.ErrorMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Throw exception while edit {_objectTypeName} : {ex}");
                    ModelState.AddModelError("", saveErrorMessage);
                }
            }

            return editView(model);
        }

        protected virtual async Task<List<ValidationResult>> checkEditData(M currentData,VM newData)
        {
            return await Task.FromResult(new List<ValidationResult>());
        }

        protected virtual IActionResult editSucceededResult(M currentData)
        {
            return RedirectToAction(nameof(Details), new { id = currentData.Id });
        }

        protected virtual IActionResult deleteView(VM currentData)
        {
            return View("Delete", currentData);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentData = await _dataManager.GetDataById(id);

                if (currentData == null)
                {
                    return NotFound();
                }

                return deleteView(_mapper.Map<VM>(currentData));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            try
            {
                _logger.LogInformation(getDeletingDataMessage(id));

                using (_dataManager)
                {
                    var currentData = await _dataManager.GetDataById(id);
                    if (currentData == null)
                    {
                        _logger.LogWarning(getDataNotFoundMessage(id));
                        return NotFound();
                    }

                    var validationResults = await checkDeleteData(currentData);
                    if (validationResults.Any())
                    {
                        foreach (var vr in validationResults)
                        {
                            foreach (var mn in vr.MemberNames)
                            {
                                _logger.LogWarning($"validation exception while deleting {_objectTypeName} :member name : {mn} error : {vr.ErrorMessage}");
                            }
                        }
                        return BadRequest();
                    }
                    else
                    {
                        var deleteResult = await _dataManager.DeleteDataItem(currentData.Id);
                        if (deleteResult.Status == RepositoryActionStatus.Deleted)
                        {
                            _logger.LogInformation(deletingDataSuccessfullMessage);
                            return deleteSucceededResult();
                        }
                        else
                        {
                            _logger.LogWarning(deletingDataFailMessage);
                            return BadRequest();
                        }
                    }
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning($"validation exception while delete {_objectTypeName} : {ex.ValidationResult.ErrorMessage}");
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Throw exception while delete {_objectTypeName} : {ex}");
                throw ex;
            }
        }

        protected virtual async Task<List<ValidationResult>> checkDeleteData(M currentData)
        {
            return await Task.FromResult(new List<ValidationResult>());
        }

        protected virtual IActionResult deleteSucceededResult()
        {
            return RedirectToAction(nameof(Index));
        }

    }
}
