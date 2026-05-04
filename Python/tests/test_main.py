from fastapi.testclient import TestClient
from main import app, analyzer
from unittest.mock import AsyncMock

client = TestClient(app)

def test_analyze_endpoint_returns_400_for_short_content():
    response = client.post("/analyze", json={"content": "abc"})
    assert response.status_code == 400
    assert "prea scurt" in response.json()["detail"]

def test_analyze_endpoint_success_logic(mocker):
    # Mock-uim metoda analyze a obiectului global analyzer
    mock_data = {"summary": "OK", "clauses": "None", "risks": "None"}
    mocker.patch.object(analyzer, 'analyze', new_callable=AsyncMock, return_value=mock_data)

    response = client.post("/analyze", json={"content": "Acesta este un document valid de peste zece caractere."})
    assert response.status_code == 200
    assert response.json()["summary"] == "OK"