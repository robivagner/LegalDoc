import pytest
from httpx import AsyncClient, ASGITransport
from main import app
from unittest.mock import AsyncMock

@pytest.mark.asyncio
async def test_full_analysis_flow_with_retry_failure(mocker):
    # Calea de mock corectă pentru clientul aio din Google GenAI
    # Trebuie să mock-uim unde este UTILIZAT, adică în analyzer
    mock_gen = mocker.patch("analyzer.genai.Client.aio", new_callable=AsyncMock)

    # Simulăm o eroare care să declanșeze logica de retry și apoi să pice
    mock_gen.models.generate_content = AsyncMock(side_effect=Exception("503 UNAVAILABLE"))

    # Folosim AsyncClient pentru a evita eroarea "Event loop is closed"
    async with AsyncClient(transport=ASGITransport(app=app), base_url="http://test") as ac:
        response = await ac.post("/analyze", json={"content": "Document lung pentru testul de integrare care va eșua."})

    assert response.status_code == 500
    assert "503" in response.json()["detail"]