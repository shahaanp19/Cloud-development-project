# CoffeeNChill

---

## Remaining Work

### 1. Verify the Storage Warning

- Investigate the `Unhealthy` WebJobs Storage warning.
- Confirm that the application can successfully connect to the configured storage service.
- Verify that Table Storage operations work correctly from the running application/container.
- Confirm that the storage configuration is suitable for the final Azure deployment.
- Ensure there are no unresolved storage health warnings before final submission.

---

### 2. Verify the Staff-Document Implementation

- Inspect the current `FileShareService` implementation.
- Confirm how staff documents are currently being stored.
- Verify whether the implementation fully satisfies the assignment requirements for Azure File Share storage.
- The current implementation was changed to use Blob Storage because Azurite does not support the required Azure File Share functionality.
- Determine whether the final Azure implementation must use an actual Azure File Share.
- Verify `UploadStaffDocument`.
- Confirm that staff document uploads use `multipart/form-data` where required.
- Test document upload.
- Test document listing.
- Test document download.
- Confirm appropriate error handling for missing or invalid documents.
- Ensure the final implementation is fully aligned with the assignment requirements.

---

### 3. Run the Supplied Postman Collection

- Import `CoffeeNChill.postman_collection.json` into Postman.
- Run the supplied requests against the application.
- Test all API endpoints.
- Verify successful responses.
- Verify expected error responses.
- Check status codes.
- Check request and response bodies.
- Record and resolve any failing requests.
- Confirm that all supplied Postman tests pass before final submission.

---

### 4. Later: Azure / CD

- Configure the final Azure environment.
- Verify the required Azure resources.
- Deploy the application to Azure.
- Confirm the deployed Functions application starts correctly.

---

# Next Person - Final Project Checks

The next person taking over the project must perform a complete final review against the assignment rubric.

## Rubric Compliance

- [ ] Obtain the latest version of the assignment rubric.
- [ ] Review every section of the rubric individually.
- [ ] Confirm that every required section has been implemented.
- [ ] Confirm that every requirement within each section has been fully completed.
- [ ] Check that no rubric requirements have been overlooked.
- [ ] Verify that the implementation matches the wording of the rubric, not just the general project requirements.
- [ ] Identify any incomplete, partially implemented, or non-compliant requirements.
- [ ] Fix any remaining rubric issues.

## Final Functional Checks

- [ ] Verify all API endpoints.
- [ ] Verify menu functionality.
- [ ] Verify menu price validation.
- [ ] Verify staff-document functionality.
- [ ] Verify storage functionality.
- [ ] Verify error handling.
- [ ] Run the complete Postman collection.
- [ ] Run all unit tests.
- [ ] Confirm all unit tests pass.
- [ ] Confirm the GitHub Actions CI pipeline passes.
- [ ] Verify the final Docker setup.
- [ ] Verify the Azure deployment.
- [ ] Verify Continuous Deployment if required.
- [ ] Test the deployed application.
- [ ] Confirm there are no unresolved warnings or errors.

## Final Submission Review

- [ ] Review the entire codebase for unfinished work.
- [ ] Remove unnecessary files or development artifacts.
- [ ] Confirm the correct files are committed to `main`.
- [ ] Confirm the repository contains the required documentation.
- [ ] Confirm screenshots/evidence required by the rubric are available.
- [ ] Confirm Postman evidence is available if required.
- [ ] Confirm unit-testing evidence is available.
- [ ] Confirm CI/CD evidence is available.
- [ ] Confirm Docker evidence is available.
- [ ] Confirm Azure deployment evidence is available.
- [ ] Perform one final review of every rubric section.
- [ ] Ensure every section is fully completed before submission.

---


